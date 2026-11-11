import { ReportsQueryParams } from '@/pages/Reports/types';
import { ReportResponseItem } from '@/types/report';

export interface UserRowProps {
	row: ReportResponseItem;
	queryParams: ReportsQueryParams;
}
