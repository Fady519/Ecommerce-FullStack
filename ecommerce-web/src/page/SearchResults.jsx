
import React, { useEffect, useState } from 'react'
import { useLocation } from 'react-router-dom';
import SlideProductLoading from './../components/slideProducts/SlideProductLoading';
import Product from './../components/slideProducts/Product';
import PageTransition from '../components/PageTransition';
import noResults from "../img/no-results.png";
import { Link } from "react-router-dom";

function SearchResults() {
    const [results, setResults] = useState([]);
    const [loading, setLoading] = useState(true);

    const query = new URLSearchParams(useLocation().search).get("query");

    useEffect(() => {
        const fetchResult = async () => {
            try {
                const res = await fetch(
                    `https://dummyjson.com/products/search?q=${query}`
                )
                const data = await res.json();
                setResults(data.products || []);
            }
            catch (error) {
                console.error("Search Error ", error);
            }
            finally {
                setLoading(false)
            }
        }
        if (query) fetchResult();
    }, [query]);

    return (
        <PageTransition key={query}>
            <div className='category_products'>
                {loading ? (
                    <SlideProductLoading key={query} />
                ) : results.length > 0 ? (
                    <div className='container'>
                        <div className='top_slide'>
                            <h2> Results For : {query} </h2>
                        </div>
                        <div className='products'>
                            {results?.map((item) => (
                                <Product item={item} key={item.id} />
                            ))}
                        </div>
                    </div>
                ) : (
                    <div className="container no-results-container">
                        <div className="no-results">
                            <img src={noResults} alt="no results" />
                            <h2>No Results Found</h2>
                            <p>
                                Sorry, we couldn't find any products for
                                <span> "{query}" </span>
                            </p>
                            <Link to="/" className="back-home">
                                Back To Home
                            </Link>
                        </div>
                    </div>
                )}
            </div>
        </PageTransition>
    )
}

export default SearchResults;


