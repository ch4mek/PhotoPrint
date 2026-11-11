import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import numeral from 'numeral';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';
import { convertFromToDateObjToQueryParam } from '../utils';

import { UserSalaryFilters } from '@/components/UserProfile/components/SalaryTab/types';
import { getUserSalaryEndpoint } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { ToastContextValues } from '@/context/types';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UserSalaryResponse } from '@/types/userInfo';

export const useUserSalaries = (
	userId: number,
	top: number,
	skip: number,
	order: SortDirection,
	orderByCondition: string,
	filters: UserSalaryFilters,
	toast: ToastContextValues
) =>
	useQueryWithCallback<UserSalaryResponse, AxiosError<ServerResponseError>>({
		queryKey: [userId, order, orderByCondition, filters, top, skip],
		queryFn: async () => {
			const { salaryFrom, salaryUsd } = filters;

			try {
				const query = buildQuery({
					filter: {
						and: [
							...convertFromToDateObjToQueryParam(salaryFrom, 'salaryFrom'),
							salaryUsd?.from &&
								`salaryUsd ge ${numeral(salaryUsd.from).value()}`,
							salaryUsd?.to && `salaryUsd le ${numeral(salaryUsd.to).value()}`,
						],
					},
					top,
					skip: skip * top,
					orderBy: `${orderByCondition} ${order}`,
				});

				const { data } = await axios.get<UserSalaryResponse>(
					`${getUserSalaryEndpoint(userId)}${query}`
				);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		onError: createOnErrorHandler(toast),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
