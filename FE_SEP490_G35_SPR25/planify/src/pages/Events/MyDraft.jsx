import React, { useState, useEffect } from "react";
import {
  Container,
  Row,
  Col,
  Card,
  Form,
  Pagination,
  Button,
  Badge,
} from "react-bootstrap";
import "../../styles/Author/HomeSpectator.css";
import Header from "../../components/Header/Header";
import { MdArrowBackIos, MdArrowForwardIos } from "react-icons/md";
import { FaHeart, FaRegHeart } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import {
  createFavoriteEvent,
  deleteFavouriteEvent,
  getMyFavouriteEvents,
  searchEvents,
} from "../../services/EventService";

export default function MyDraft() {
  const [events, setEvents] = useState([]);
  const navigate = useNavigate();
  const pageSize = 8;
  const [currentPage, setCurrentPage] = useState(1);
  const [totalEvents, setTotalEvents] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [loading, setLoading] = useState(true);
  const [categories, setCategories] = useState([]);
  const [locations, setLocations] = useState([]);

  const fixDriveUrl = (url) => {
    if (!url || typeof url !== "string")
      return "https://placehold.co/600x400?text=No+Image";
    if (!url.includes("drive.google.com/uc?id=")) return url;
    const fileId = url.split("id=")[1];
    return `https://drive.google.com/thumbnail?id=${fileId}&sz=w1000`;
  };

  const getImageUrl = (eventMedias) => {
    if (!eventMedias || !eventMedias.length || !eventMedias[0]) {
      return "https://placehold.co/600x400?text=No+Image";
    }
    return fixDriveUrl(eventMedias[0].mediaUrl);
  };
  const handleCreateFavorite = async (eventId, e) => {
    try {
      e.stopPropagation();
      var response = await createFavoriteEvent(eventId);
      if (response.status === 201) {
        console.log("Đã thêm sự kiện vào danh sách yêu thích:", eventId);
        setEvents((prevEvents) =>
          prevEvents.map((event) =>
            event.id === eventId ? { ...event, isFavorite: true } : event
          )
        );
      }
    } catch (error) {
      console.error("Lỗi khi thêm sự kiện vào yêu thích:", error);
    }
  };

  const handleDeleteFavorite = async (eventId, e) => {
    try {
      e.stopPropagation();
      var response = await deleteFavouriteEvent(eventId);
      if (response.status === 200) {
        console.log("Đã xóa sự kiện khỏi danh sách yêu thích:", eventId);
        setEvents((prevEvents) =>
          prevEvents.map((event) =>
            event.id === eventId ? { ...event, isFavorite: false } : event
          )
        );
      }
    } catch (error) {
      console.error("Lỗi khi xóa sự kiện khỏi yêu thích:", error);
    }
  };
  const fetchEvents = async () => {
    try {
      setLoading(true);
      const params = {
        page: currentPage,
        pageSize: pageSize,
        status: "0",
        createBy: localStorage.getItem("userId"),
      };
      const response = await searchEvents(params);

      if (response && response.items) {
        setEvents(response.items);
        setTotalEvents(response.totalCount || 0);
        setTotalPages(response.totalPages || 1);

        if (categories.length === 0 || locations.length === 0) {
          extractCategoriesAndLocations(response.items);
        }
      } else {
        console.error("Unexpected API response format:", response);
        setEvents([]);
      }
    } catch (error) {
      console.error("Error fetching events:", error);
      setEvents([]);
    } finally {
      setLoading(false);
    }
  };

  const extractCategoriesAndLocations = (eventsData) => {
    if (!eventsData || !Array.isArray(eventsData)) return;

    const categoriesSet = new Set();
    const locationsSet = new Set();

    eventsData.forEach((event) => {
      if (
        event.categoryViewModel &&
        event.categoryViewModel.categoryEventName
      ) {
        categoriesSet.add(event.categoryViewModel.categoryEventName);
      }
      if (event.placed) {
        locationsSet.add(event.placed);
      }
    });

    setCategories(Array.from(categoriesSet));
    setLocations(Array.from(locationsSet));
  };

  useEffect(() => {
    fetchEvents();
  }, [currentPage]);

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
    window.scrollTo(0, 0);
  };

  return (
    <>
      <Header />
      <div className="d-flex">
        <div className="flex-grow-1 px-4" style={{ marginTop: "100px" }}>
          <Container fluid className="px-2">
            <div style={{ maxWidth: "1440px", margin: "0 auto" }}>
              {loading ? (
                <div className="text-center py-5">
                  <div className="spinner-border" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                </div>
              ) : (
                <>
                  <Row xs={1} sm={2} md={3} lg={4} className="g-4">
                    {events.length > 0 ? (
                      events.map((event) => (
                        <Col key={event.id}>
                          <Card
                            className="h-100 shadow-sm event-card"
                            onClick={() =>
                              navigate(`/event-detail-EOG/${event.id}`, {
                                state: { from: "my-drafts" },
                              })
                            }
                            style={{ cursor: "pointer", position: "relative" }}
                          >
                            <Card.Img
                              variant="top"
                              src={
                                event.eventMedias &&
                                event.eventMedias.length > 0 &&
                                event.eventMedias[0] &&
                                event.eventMedias[0].mediaDTO &&
                                event.eventMedias[0].mediaDTO.mediaUrl
                                  ? fixDriveUrl(
                                      event.eventMedias[0].mediaDTO.mediaUrl
                                    )
                                  : "https://placehold.co/600x400?text=No+Image"
                              }
                              height="180"
                              className="event-image"
                              style={{ objectFit: "cover" }}
                              onError={(e) => {
                                e.target.src =
                                  "https://placehold.co/600x400?text=No+Image";
                              }}
                            />
                            <div
                              className="favorite-button"
                              style={{
                                position: "absolute",
                                top: "190px",
                                right: "20px",
                                zIndex: 10,
                                // background: "rgba(255,255,255,0.7)",
                                borderRadius: "50%",
                                padding: "5px",
                                cursor: "pointer",
                              }}
                              onClick={(e) =>
                                event.isFavorite
                                  ? handleDeleteFavorite(event.id, e)
                                  : handleCreateFavorite(event.id, e)
                              }
                            >
                              {event.isFavorite ? (
                                <FaHeart size={20} color="red" />
                              ) : (
                                <FaRegHeart size={20} color="red" />
                              )}
                            </div>

                            <Card.Body>
                              <span
                                className={`status-badge ${
                                  event.statusMessage === "Đang diễn ra"
                                    ? "status-running"
                                    : event.statusMessage === "Đã kết thúc"
                                    ? "status-closed"
                                    : "status-notyet"
                                }`}
                              >
                                {event.statusMessage === "Đang diễn ra"
                                  ? "Đang diễn ra"
                                  : event.statusMessage === "Đã kết thúc"
                                  ? "Đã kết thúc"
                                  : "Chưa bắt đầu"}
                              </span>
                              <Card.Title
                                style={{ fontSize: "100%" }}
                                className="event-title fw-bold"
                              >
                                {event.eventTitle}
                              </Card.Title>
                              <div>
                                <div>
                                  <small className="text-muted">
                                    {new Date(event.startTime).toLocaleString(
                                      "vi",
                                      {
                                        weekday: "short",
                                        month: "short",
                                        day: "numeric",
                                        hour: "numeric",
                                        minute: "2-digit",
                                      }
                                    )}
                                  </small>
                                </div>
                                <div>
                                  <small className="text-muted">
                                    Địa điểm: {event.placed}
                                  </small>
                                </div>
                                {event.categoryViewModel && (
                                  <div>
                                    <small className="text-muted">
                                      Loại sự kiện:{" "}
                                      {
                                        event.categoryViewModel
                                          .categoryEventName
                                      }
                                    </small>
                                  </div>
                                )}
                              </div>
                            </Card.Body>
                          </Card>
                        </Col>
                      ))
                    ) : (
                      <Col xs={12}>
                        <div
                          className="d-flex flex-column justify-content-center align-items-center"
                          style={{ height: "300px" }}
                        >
                          <p className="text-muted text-center">
                            {totalEvents === 0
                              ? "Không có sự kiện nào có sẵn."
                              : "Không tìm thấy sự kiện nào phù hợp với tiêu chí của bạn."}
                          </p>
                        </div>
                      </Col>
                    )}
                  </Row>

                  {totalPages > 1 && (
                    <div className="d-flex justify-content-center mt-4">
                      <Pagination>
                        <Pagination.First
                          onClick={() => handlePageChange(1)}
                          disabled={currentPage === 1}
                        />
                        <Pagination.Prev
                          onClick={() => handlePageChange(currentPage - 1)}
                          disabled={currentPage === 1}
                        />

                        {Array.from({ length: totalPages }, (_, i) => i + 1)
                          .filter(
                            (page) =>
                              page === 1 ||
                              page === totalPages ||
                              (page >= currentPage - 1 &&
                                page <= currentPage + 1)
                          )
                          .map((page, index, array) => {
                            if (index > 0 && array[index - 1] !== page - 1) {
                              return [
                                <Pagination.Ellipsis
                                  key={`ellipsis-${page}`}
                                  disabled
                                />,
                                <Pagination.Item
                                  key={page}
                                  active={page === currentPage}
                                  onClick={() => handlePageChange(page)}
                                >
                                  {page}
                                </Pagination.Item>,
                              ];
                            }
                            return (
                              <Pagination.Item
                                key={page}
                                active={page === currentPage}
                                onClick={() => handlePageChange(page)}
                              >
                                {page}
                              </Pagination.Item>
                            );
                          })}

                        <Pagination.Next
                          onClick={() => handlePageChange(currentPage + 1)}
                          disabled={currentPage === totalPages}
                        />
                        <Pagination.Last
                          onClick={() => handlePageChange(totalPages)}
                          disabled={currentPage === totalPages}
                        />
                      </Pagination>
                    </div>
                  )}
                </>
              )}
            </div>
          </Container>
        </div>
      </div>
    </>
  );
}
