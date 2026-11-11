import { ReactNode } from 'react';

import { UserAbsenceItem } from '@/types/absences';

export interface UserAbsenceCardProps {
	absenceItem: UserAbsenceItem;
	width?: string | number;
	actions?: ReactNode;
}
