interface PaginationProps {
  currentPage: number;
  setCurrentPage: (page: number) => void;
  totalPages: number;
}

export default function Pagination({ currentPage, setCurrentPage, totalPages }: PaginationProps) {

    const pageRange = [];
    for (let i = 1; i <= totalPages; i++) {
        pageRange.push(i);
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
                        <a href="#" onClick={() => setCurrentPage(page)}>{page}</a>
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
                    <a href="#" title="First"
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
                    <a href="#" title="Previous"
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
                    <a href="#" title="Next"
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
                    <a href="#" title="Last"
                        className="fa fa-fast-forward"
                        onClick={() => setCurrentPage(totalPages)}></a>
                )
            }
            {/* <a href="#" title="Last" className="fa fa-fast-forward"></a>         */}
        </span>
    </div>
    </>
  );
}