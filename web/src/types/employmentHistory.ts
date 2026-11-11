import { Dayjs } from '@/lib/dayjs';

export interface EmploymentHistoryResponse {
	professionalActivityStartDate: Dayjs | null;
	firstProjectStartDate: Dayjs | null;
	hireDate: Dayjs | null;
	contractEndDate: Dayjs | null;
	fireDate: Dayjs | null;
	ndaStartDate?: Dayjs | null;
	ndaEndDate?: Dayjs | null;
	shouldDisplayNda: boolean;
}
