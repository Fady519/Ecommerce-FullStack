import React, { useState, useEffect } from "react";
import "./Accessories.css";
import { Link } from "react-router-dom";

// استيراد الصور محليًا
import acc1 from "../../img/acc1.jpg";
import acc2 from "../../img/acc2.png";
import acc3 from "../../img/acc3.jpg";
import acc4 from "../../img/acc4.jpg";
import acc5 from "../../img/acc5.jpg";
import acc6 from "../../img/acc6.jpg";

function Accessories() {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    const dummyProducts = [
      { id: 1, title: "Wireless Mouse", price: "$25", image: acc1 },
      { id: 2, title: "Gaming Headset", price: "$55", image: acc2 },
      { id: 3, title: "Keyboard RGB", price: "$45", image: acc3 },
      { id: 4, title: "USB Hub", price: "$20", image: acc4 },
      { id: 5, title: "Laptop Stand", price: "$35", image: acc5 },
      { id: 6, title: "Webcam HD", price: "$60", image: acc6 },
    ];
    setProducts(dummyProducts);
  }, []);

  return (
    <div className="accessories-container">
      <h1 className="accessories-title">Accessories</h1>
      <div className="accessories-grid">
        {products.map((product) => (
          <div key={product.id} className="accessory-card">
            <img src={product.image} alt={product.title} />
            <h3>{product.title}</h3>
            <p className="price">{product.price}</p>
            <Link to="/" className="btn">Shop Now</Link>
          </div>
        ))}
      </div>
    </div>
  );
}

export default Accessories;