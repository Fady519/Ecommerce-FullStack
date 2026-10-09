import React from "react";
import { useParams, Link } from "react-router-dom";
import "./Blog.css";

// استيراد الصور محليًا
import shoppingImg from "../../img/shopping.jpg";
import qualityImg from "../../img/quality.jpg";
import trendsImg from "../../img/trends.jpg";

function BlogDetail() {
  const { id } = useParams();

  const postsData = {
    1: {
      title: "Smart Shopping Tips for 2026",
      date: "March 10, 2026",
      content: "Online shopping is about strategy. Use wishlists, compare reviews, check product authenticity, and look for coupons before buying...",
      image: shoppingImg,
    },
    2: {
      title: "How to Choose Quality Products",
      date: "March 9, 2026",
      content: "Picking quality products starts with research. Look at materials, check warranty options, and read customer experiences...",
      image: qualityImg,
    },
    3: {
      title: "E-commerce Trends You Should Know",
      date: "March 8, 2026",
      content: "In 2026, personalized recommendation systems and fast delivery are major parts of eCommerce growth...",
      image: trendsImg,
    },
  };

  const post = postsData[id];

  if (!post) {
    return (
      <div className="blog-container">
        <h2>Article not found</h2>
        <Link to="/blog">Back to Blog</Link>
      </div>
    );
  }

  return (
    <div className="blog-container">
      <h1 className="blog-title">{post.title}</h1>
      <img src={post.image} alt={post.title} className="blog-detail-image"/>
      <p className="blog-date">{post.date}</p>
      <p className="blog-content">{post.content}</p>
      <Link to="/blog" className="back-link">← Back to Posts</Link>
    </div>
  );
}

export default BlogDetail;