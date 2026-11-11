import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';

import { AbsenceRow } from '@/types/absences';

export interface AbsenceTableProps {
	pagination?: GridPaginationProps;
	rows: Array<AbsenceRow>;
	isLoading?: boolean;
}
