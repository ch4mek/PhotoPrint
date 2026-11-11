import axios, { AxiosError } from 'axios';
import { useCallback, useRef, useState } from 'react';

import { DateFormat } from '@/constants/date';
import { requestQueriesKeys } from '@/constants/queriesKeys';
import { GET_SELECTED_DAYS_INQUIRIES_ENDPOINT } from '@/constants/queryPaths';
import { formatDate } from '@/helpers/formatDate';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { RequestSelectedDays, RequestSelectedParam } from './types';

const isValidParams = (params: RequestSelectedParam | null): boolean => {
	if (!params) {
		return false;
	}

	const { templateId, userId, dateFrom, dateTo } = params;

	return !!(templateId && userId && dateFrom && dateTo);
};

const areParamsEqual = (
	prev: RequestSelectedParam | null,
	next: RequestSelectedParam
): boolean => {
	if (!prev) {
		return false;
	}

	const prevDateFrom =
		prev.dateFrom && formatDate(prev.dateFrom, DateFormat.YYYY_MM_DD);
	const prevDateTo =
		prev.dateTo && formatDate(prev.dateTo, DateFormat.YYYY_MM_DD);
	const nextDateFrom =
		next.dateFrom && formatDate(next.dateFrom, DateFormat.YYYY_MM_DD);
	const nextDateTo =
		next.dateTo && formatDate(next.dateTo, DateFormat.YYYY_MM_DD);

	return (
		prev.templateId === next.templateId &&
		prev.userId === next.userId &&
		prevDateFrom === nextDateFrom &&
		prevDateTo === nextDateTo
	);
};

export const useGetRequestSelectedDays = () => {
	const [params, setParams] = useState<RequestSelectedParam | null>(null);
	const lastParamsRef = useRef<RequestSelectedParam | null>(null);

	const { data, isLoading, error } = useQueryWithCallback<
		RequestSelectedDays,
		AxiosError
	>({
		queryKey: [requestQueriesKeys.getRequestSelectedDays, params],
		queryFn: async () => {
			if (!params) {
				return { selectedDays: 0 };
			}

			const { templateId, userId, dateFrom, dateTo } = params;

			return axios
				.get<RequestSelectedDays>(`${GET_SELECTED_DAYS_INQUIRIES_ENDPOINT}`, {
					params: {
						templateId,
						userId,
						dateFrom: dateFrom && formatDate(dateFrom, DateFormat.YYYY_MM_DD),
						dateTo: dateTo && formatDate(dateTo, DateFormat.YYYY_MM_DD),
					},
				})
				.then((res) => res.data)
				.catch(handleRequestError);
		},
		enabled: isValidParams(params),
		refetchOnWindowFocus: false,
		staleTime: 0,
	});

	const getRequestSelectedDays = useCallback((filter: RequestSelectedParam) => {
		if (areParamsEqual(lastParamsRef.current, filter)) {
			return;
		}

		lastParamsRef.current = filter;
		setParams(filter);
	}, []);

	return {
		getRequestSelectedDays,
		data: data?.selectedDays ?? null,
		isLoading,
		error,
	};
};
