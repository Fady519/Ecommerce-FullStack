

import React, { useEffect, useState } from 'react'
import { FaSearch } from "react-icons/fa";
import { Link, useLocation, useNavigate } from 'react-router-dom';

function SearchBox() {
    const navigate = useNavigate();
    const location = useLocation();

    const [searchTerm, setSearchTerm] = useState("");
    const [suggestation, setSuggestation] = useState([]);

    
    useEffect(() => {
        const queryParam = new URLSearchParams(location.search).get("query") || "";
        if(location.pathname.startsWith("/search")) {
            setSearchTerm(queryParam);
        } else {
            setSearchTerm(""); 
        }
    }, [location]);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (searchTerm.trim()) {
            navigate(`/search?query=${encodeURIComponent(searchTerm.trim())}`);
        }
        setSuggestation([]);
    }

    // fetch suggestions
    useEffect(() => {
        const fetchSuggestation = async () => {
            if (!searchTerm.trim()) {
                setSuggestation([]);
                return;
            }
            try {
                const res = await fetch(
                    `https://dummyjson.com/products/search?q=${searchTerm}`
                );
                const data = await res.json();
                setSuggestation(data.products.slice(0, 5) || []);
            } catch (error) {
                console.error("Search Error : ", error);
                setSuggestation([]);
            }
        }

        const debonuse = setTimeout(() => {
            fetchSuggestation();
        }, 300);

        return () => clearTimeout(debonuse);
    }, [searchTerm]);

    return (
        <div className='searchBox_container'>
            <form onSubmit={handleSubmit} className='search_box'>
                <input
                    type="text"
                    name='search'
                    id='search'
                    placeholder='Search For Products'
                    value={searchTerm}  
                    onChange={(e) => setSearchTerm(e.target.value)}
                    autoComplete='off'
                />
                <button type='submit'> <FaSearch /> </button>
            </form>

            {suggestation.length > 0 && (
                <ul className='suggestation'>
                    {suggestation.map((item) => (
                        <Link key={item.id} to={`/products/${item.id}`}>
                            <li>
                                <img src={item.images[0]} alt={item.title} />
                                <span>{item.title}</span>
                            </li>
                        </Link>
                    ))}
                </ul>
            )}
        </div>
    )
}

export default SearchBox;