import { useMemo } from 'react';

import { UserCertificatesRow } from '../types';

import { DateFormat } from '@/constants/date';
import { useConfiguration } from '@/context/Configuration';
import dayjs from '@/lib/dayjs';
import { Certificate } from '@/types/certificate';

export function useCertificatesRows(data?: Certificate[]) {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const rows: UserCertificatesRow[] = useMemo(() => {
		if (!data || !data.length) {
			return [];
		}

		return data.map(
			({
				certificateId,
				certificateImageURL,
				certificateViewLink,
				technology,
				stack,
				receiptDate,
			}) => ({
				certificateId,
				certificateImageURL,
				technology:
					departmentTypes.find(({ id }) => id === technology)?.fullName || '',
				certificateViewLink: certificateViewLink ?? '',
				stack: stack ?? '',
				receiptDate: dayjs(receiptDate).format(DateFormat.DD_MM_YYYY),
			})
		);
	}, [data]);

	return rows;
}
