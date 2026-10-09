import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import Product from './../../components/slideProducts/Product';
import './CategoryPage.css'
import SlideProductLoading from './../../components/slideProducts/SlideProductLoading';
import PageTransition from '../../components/PageTransition';



function CategoryPage() {

    const { category } = useParams();

    const [loading, setLoading] = useState(false);
    const [categoryProducts, setCategoryProducts] = useState([]);

    useEffect(() => {
        setLoading(true);

        fetch(`https://dummyjson.com/products/category/${category}`)
            .then((res) => res.json())
            .then((data) => {
                setCategoryProducts(data.products);
            })
            .catch((error) => console.error(error))
            .finally(() => setLoading(false));

    }, [category]);

    return (
        <PageTransition key={category}>
            <div className='category_products'>

                {loading ? (
                    <SlideProductLoading />
                ) : (
                    <div className='container'>

                        <div className='top_slide'>
                            <h2>{category} : {categoryProducts.length}</h2>
                            <p>Lorem ipsum dolor sit amet consectetur adipisicing elit.</p>
                        </div>

                        <div className='products'>
                            {categoryProducts?.map((item) => (
                                <Product item={item} key={item.id} />
                            ))}
                        </div>

                    </div>
                )}

            </div>
        </PageTransition>
    )
}

export default CategoryPage
