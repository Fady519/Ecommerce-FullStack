import React, { useState } from "react";
import "./Contact.css";
import PageTransition from "../../components/PageTransition";
import { toast } from "react-hot-toast";
import emailjs from "@emailjs/browser";

function Contact() {

  const [formData, setFormData] = useState({ name:"", email:"", message:"" });
  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    setFormData({...formData, [e.target.name]: e.target.value});
  };

  // Validation
  const validate = () => {
    const newErrors = {};
    if(!formData.name.trim()) newErrors.name = "Name is required";
    if(!formData.email.trim()) newErrors.email = "Email is required";
    else if(!/\S+@\S+\.\S+/.test(formData.email)) newErrors.email = "Email is invalid";
    if(!formData.message.trim()) newErrors.message = "Message is required";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }

  // إرسال الرسالة
  const handleSubmit = (e) => {
    e.preventDefault();
    if(!validate()) return;

    emailjs.send(
      "service_8f3px09",      // من EmailJS
      "template_tgmu5da",     // من EmailJS
      formData,
      "zT_B3bOgxKo2IqQdk"       // User ID من EmailJS
    )
    .then(() => {
      toast.success("Message sent successfully!");
      setFormData({ name:"", email:"", message:"" });
    })
    .catch((err) => {
      toast.error("Failed to send message. Try again!");
      console.error(err);
    });
  }

  return (
    <PageTransition>
      <div className="contact-page">
        <div className="contact-hero">
          <div className="container">
            <h1>Contact Us</h1>
            <p>We are here to help you. Send us a message and we will reply as soon as possible.</p>
          </div>
        </div>

        <div className="contact-content container">

          <div className="contact-form">
            <h2>Get in Touch</h2>
            <form onSubmit={handleSubmit}>

              <div className="form-group">
                <label>Name</label>
                <input type="text" name="name" value={formData.name} onChange={handleChange} />
                {errors.name && <span className="error">{errors.name}</span>}
              </div>

              <div className="form-group">
                <label>Email</label>
                <input type="email" name="email" value={formData.email} onChange={handleChange} />
                {errors.email && <span className="error">{errors.email}</span>}
              </div>

              <div className="form-group">
                <label>Message</label>
                <textarea name="message" rows="5" value={formData.message} onChange={handleChange}></textarea>
                {errors.message && <span className="error">{errors.message}</span>}
              </div>

              <button type="submit">Send Message</button>
            </form>
          </div>

          <div className="contact-info">
            <h2>Contact Info</h2>
            <p><strong>Phone:</strong> +201575765</p>
            <p><strong>Email:</strong> fadykaiser2004@gmail.com</p>
            <p><strong>Address:</strong> Egypt Cario</p>
          </div>

        </div>

      </div>
    </PageTransition>
  );
}

export default Contact;