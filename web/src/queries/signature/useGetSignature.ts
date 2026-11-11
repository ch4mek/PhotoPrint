import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { SignatureQueriesKeys } from '@/constants/queriesKeys';
import { GET_SIGNATURE } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

export const useGetSignature = (
	toast: (message: string, type: NotificationType) => void
) =>
	useQueryWithCallback({
		queryKey: [SignatureQueriesKeys.signature],
		queryFn: async () => {
			try {
				const { data } = await axios.get(GET_SIGNATURE);

				return data;
			} catch (error) {
				toast(SOMETHING_WENT_WRONG, NotificationType.ERROR);

				return undefined;
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
