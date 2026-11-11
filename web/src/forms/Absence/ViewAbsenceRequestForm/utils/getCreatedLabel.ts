import { SHORT_MONTH_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const getCreatedLabel = (createdDate: string) =>
	`Created ${dayjs(createdDate).format(SHORT_MONTH_DATE_FORMAT)}`;
