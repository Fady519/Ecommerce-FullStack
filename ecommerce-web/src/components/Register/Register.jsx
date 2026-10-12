import React, { useContext, useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { AuthContext } from '../Context/AuthContext';
import { ApiError } from '../../services/api';
import '../Login/Login.css';

export default function Register() {
  const { register: registerField, handleSubmit, watch, setError, formState: { errors } } = useForm();
  const password = watch("password");
  const navigate = useNavigate();
  const { register: registerUser } = useContext(AuthContext);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const onSubmit = async (data) => {
    setIsSubmitting(true);
    try {
      const user = await registerUser(data.fullName, data.email, data.password, data.phoneNumber || "01000000000");
      toast.success(`Account created! Welcome, ${user.displayName}!`);
      navigate("/", { replace: true });
    } catch (err) {
      if (err instanceof ApiError && err.errors) {
        // Map backend validation errors directly under their respective input fields
        const backendErrors = err.errors;
        let passwordErrorMessages = [];

        Object.keys(backendErrors).forEach((key) => {
          const messages = backendErrors[key];
          const lowerKey = key.toLowerCase();

          if (lowerKey.includes('password')) {
            passwordErrorMessages.push(...messages);
          } else if (lowerKey.includes('email')) {
            setError('email', { type: 'server', message: messages[0] });
          } else if (lowerKey.includes('name') || lowerKey.includes('display')) {
            setError('fullName', { type: 'server', message: messages[0] });
          } else if (lowerKey.includes('phone')) {
            setError('phoneNumber', { type: 'server', message: messages[0] });
          }
        });

        if (passwordErrorMessages.length > 0) {
          setError('password', { type: 'server', message: passwordErrorMessages.join(' • ') });
        }
      } else {
        toast.error(err.message || "Registration failed. Please check your data.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="auth_container">
      <div className="auth_card">
        <h2>Create Account</h2>
        <p>Join us today! It only takes a minute.</p>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="input_group">
            <label>Full Name</label>
            <input 
              type="text" 
              placeholder="Fady Kaiser"
              className={errors.fullName ? "input_error" : ""}
              {...registerField("fullName", { required: "Full name is required" })} 
            />
            {errors.fullName && <span className="error_msg">{errors.fullName.message}</span>}
          </div>

          <div className="input_group">
            <label>Email Address</label>
            <input 
              type="email" 
              placeholder="example@mail.com"
              className={errors.email ? "input_error" : ""}
              {...registerField("email", { 
                required: "Email is required", 
                pattern: { value: /^\S+@\S+$/i, message: "Invalid email address" } 
              })} 
            />
            {errors.email && <span className="error_msg">{errors.email.message}</span>}
          </div>

          <div className="input_group">
            <label>Phone Number (Optional)</label>
            <input 
              type="tel" 
              placeholder="01012345678"
              className={errors.phoneNumber ? "input_error" : ""}
              {...registerField("phoneNumber")} 
            />
            {errors.phoneNumber && <span className="error_msg">{errors.phoneNumber.message}</span>}
          </div>

          <div className="input_group">
            <label>Password</label>
            <input 
              type="password" 
              placeholder="••••••••"
              className={errors.password ? "input_error" : ""}
              {...registerField("password", { 
                required: "Password is required", 
                minLength: { value: 6, message: "Min length is 6 characters" } 
              })} 
            />
            <small style={{ display: 'block', color: '#777', fontSize: '11px', marginTop: '4px' }}>
              Must have at least 8 characters, uppercase, lowercase, and numbers.
            </small>
            {errors.password && <span className="error_msg">{errors.password.message}</span>}
          </div>

          <div className="input_group">
            <label>Confirm Password</label>
            <input 
              type="password" 
              placeholder="••••••••"
              className={errors.confirmPassword ? "input_error" : ""}
              {...registerField("confirmPassword", { 
                required: "Please confirm your password",
                validate: value => value === password || "Passwords do not match"
              })} 
            />
            {errors.confirmPassword && <span className="error_msg">{errors.confirmPassword.message}</span>}
          </div>

          <button type="submit" className="main_btn" disabled={isSubmitting}>
            {isSubmitting ? "Creating account..." : "Create Account"}
          </button>
        </form>

        <div className="auth_footer">
          <p>Already have an account? <Link to="/login">Login here</Link></p>
        </div>
      </div>
    </div>
  );
}