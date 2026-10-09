import './about.css'


import React from "react";
import "./About.css";
import PageTransition from '../../components/PageTransition';


function About() {
  return (
    <PageTransition>
      <div className="about-page">

        <div className="about-hero">
          <div className="container">
            <h1>About Our Store</h1>
            <p>
              We provide high quality products with affordable prices and
              the best online shopping experience.
            </p>
          </div>
        </div>

        <div className="about-content container">

          <div className="about-text">
            <h2>Our Story</h2>
            <p>
              Our store was created to make online shopping simple and enjoyable.
              We focus on delivering high-quality products, fast shipping,
              and excellent customer service for our customers.
            </p>
          </div>

          <div className="about-features">

            <div className="feature">
              <h3>Fast Delivery</h3>
              <p>We deliver your products quickly and safely.</p>
            </div>

            <div className="feature">
              <h3>Secure Payment</h3>
              <p>Your payments are protected with secure systems.</p>
            </div>

            <div className="feature">
              <h3>Premium Quality</h3>
              <p>All our products are selected carefully.</p>
            </div>

          </div>

          <div className="about-stats">

            <div className="stat">
              <h3>5000+</h3>
              <p>Happy Customers</p>
            </div>

            <div className="stat">
              <h3>1200+</h3>
              <p>Products</p>
            </div>

            <div className="stat">
              <h3>24/7</h3>
              <p>Support</p>
            </div>

          </div>

        </div>

      </div>
    </PageTransition>
  );
}

export default About;

