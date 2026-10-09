import React, { useRef, useState } from 'react';
// Import Swiper React components
import { Swiper, SwiperSlide } from 'swiper/react';

// Import Swiper styles
import 'swiper/css';
import 'swiper/css/pagination';


import banner1 from '../../src/img/banner_Hero1.jpg'
import banner2 from '../../src/img/banner_Hero2.jpg';
import banner3 from '../../src/img/banner_Hero3.jpg';

// import required modules
import { Autoplay, Pagination } from 'swiper/modules';
import { Link } from 'react-router-dom';


function HeroSlider() {
    return (
        <>
            <div className='container'>
                <Swiper loop={true} autoplay={{
                    delay: 2500,
                    disableOnInteraction: false,
                }} pagination={true} modules={[Pagination, Autoplay]} className="mySwiper">

                    <SwiperSlide>
                        <div className='content'>
                            <h4> Limited Edition </h4>
                            <h3> Premium <br /> Smartphones </h3>
                            <p> Experience the future with our latest mobile collection </p>
                            <Link to="/" className='btn' > Shop Now </Link>
                        </div>
                        <img src={banner1} alt='slider hero 1' />
                    </SwiperSlide>

                    <SwiperSlide>
                        <div className='content'>
                            <h4> Exclusive Collection </h4>
                            <h3> Luxury <br /> Watches & Styles </h3>
                            <p> Modern designs that define your personality </p>
                            <Link to="/" className='btn' > Explore More</Link>
                        </div>
                        <img src={banner2} alt='slider hero 2' />
                    </SwiperSlide>


                    <SwiperSlide>
                        <div className='content'>
                            <h4> Mega Sale </h4>
                            <h3> Tech & <br /> Laptop Deals </h3>
                            <p> Up to 50% off on selected computing devices </p>
                            <Link to="/" className='btn' > Get Offers </Link>
                        </div>
                        <img src={banner3} alt='slider hero 3' />
                    </SwiperSlide>

                </Swiper>
            </div>
        </>
    )
}

export default HeroSlider
