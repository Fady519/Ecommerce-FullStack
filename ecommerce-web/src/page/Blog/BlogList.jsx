import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "./Blog.css";

// استيراد الصور محليًا
import shoppingImg from "../../img/shopping.jpg";
import qualityImg from "../../img/quality.jpg";
import trendsImg from "../../img/trends.jpg";

function BlogList() {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    const dummyPosts = [
      {
        id: 1,
        title: "Smart Shopping Tips for 2026",
        date: "March 10, 2026",
        summary: "Discover the best strategies to shop smart and save money online.",
        image: shoppingImg,
      },
      {
        id: 2,
        title: "How to Choose Quality Products",
        date: "March 9, 2026",
        summary: "A beginner's guide on picking high-quality products without overspending.",
        image: qualityImg,
      },
      {
        id: 3,
        title: "E-commerce Trends You Should Know",
        date: "March 8, 2026",
        summary: "Explore upcoming trends in online shopping this year.",
        image: trendsImg,
      },
    ];
    setPosts(dummyPosts);
  }, []);

  return (
    <div className="blog-container">
      <h1 className="blog-title">Latest Articles</h1>
      <div className="blog-grid">
        {posts.map((post) => (
          <div key={post.id} className="blog-card">
            <img src={post.image} alt={post.title} className="blog-image"/>
            <h2><Link to={`/blog/${post.id}`}>{post.title}</Link></h2>
            <p className="blog-date">{post.date}</p>
            <p>{post.summary}</p>
            <Link to={`/blog/${post.id}`} className="read-more">Read More →</Link>
          </div>
        ))}
      </div>
    </div>
  );
}

export default BlogList;