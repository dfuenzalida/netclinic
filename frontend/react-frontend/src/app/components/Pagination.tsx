import { PaginationProps } from "../../types/types";
import T from './Translations';

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
        <span>{T("pages")}</span>
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
                    <span title={T("first")} className="fa fa-fast-backward"></span>
                ) : (
                    <a href={`${linkBase}?page=1`} title={T("first")}
                        className="fa fa-fast-backward"
                        onClick={() => setCurrentPage(1)}></a>
                )
            }
        </span>
        <span>
        &nbsp;
            {
                currentPage === 1 ? (
                    <span title={T("previous")} className="fa fa-step-backward"></span>
                ) : (
                    <a href={`${linkBase}?page=${currentPage - 1}`} title={T("previous")}
                        className="fa fa-step-backward"
                        onClick={() => setCurrentPage(currentPage - 1)}></a>
                )
            }
        </span>

        <span>
            &nbsp;
            {
                currentPage === totalPages ? (
                    <span title={T("next")} className="fa fa-step-forward"></span>
                ) : (
                    <a href={`${linkBase}?page=${currentPage + 1}`} title={T("next")}
                        className="fa fa-step-forward"
                        onClick={() => setCurrentPage(currentPage + 1)}></a>
                )
            }
        </span>

        <span>
            &nbsp;
            {
                currentPage === totalPages ? (
                    <span title={T("last")} className="fa fa-fast-forward"></span>
                ) : (
                    <a href={`${linkBase}?page=${totalPages}`} title={T("last")}
                        className="fa fa-fast-forward"
                        onClick={() => setCurrentPage(totalPages)}></a>
                )
            }
        </span>
    </div>
    </>
  );
}