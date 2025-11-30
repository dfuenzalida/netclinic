import Link from 'next/link';
import { PaginationProps } from "../types/Types";
import page from '../page';

export default function Pagination({ currentPage, setCurrentPage, totalPages, linkBase }: PaginationProps) {

    const pageRange = [];
    for (let i = 1; i <= totalPages; i++) {
        pageRange.push(i);
    }

  if (totalPages === 1) {
    return <span></span>; // No pagination needed for a single page
  }

  return (
    <>
    <div>
        <span>pages</span>
            &nbsp;
            <span>[</span>
            &nbsp;

            {pageRange.map((page) => (
                <span key={page}>
                    {(currentPage !== page) ? (
                        <a href={`${linkBase}?page=${page}`} onClick={() => setCurrentPage(page)}>{page}</a>
                    ) : (
                        <span>{page}</span>
                    )}
                    &nbsp;
                </span>
            ))}

            <span>
            ]&nbsp;
 
        </span>

        <span>
            &nbsp;
            {
                currentPage === 1 ? (
                    <span title="First" className="fa fa-fast-backward"></span>
                ) : (
                    <a href={`${linkBase}?page=1`} title="First"
                        className="fa fa-fast-backward"
                        onClick={() => setCurrentPage(1)}></a>
                )
            }
        </span>
        <span>
        &nbsp;
            {
                currentPage === 1 ? (
                    <span title="Previous" className="fa fa-step-backward"></span>
                ) : (
                    <a href={`${linkBase}?page=${currentPage - 1}`} title="Previous"
                        className="fa fa-step-backward"
                        onClick={() => setCurrentPage(currentPage - 1)}></a>
                )
            }
        </span>

        <span>
            &nbsp;
            {
                currentPage === totalPages ? (
                    <span title="Next" className="fa fa-step-forward"></span>
                ) : (
                    <a href={`${linkBase}?page=${currentPage + 1}`} title="Next"
                        className="fa fa-step-forward"
                        onClick={() => setCurrentPage(currentPage + 1)}></a>
                )
            }
        </span>

        <span>
            &nbsp;
            {
                currentPage === totalPages ? (
                    <span title="Last" className="fa fa-fast-forward"></span>
                ) : (
                    <a href={`${linkBase}?page=${totalPages}`} title="Last"
                        className="fa fa-fast-forward"
                        onClick={() => setCurrentPage(totalPages)}></a>
                )
            }
        </span>
    </div>
    </>
  );
}