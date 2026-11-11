import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { convertFilterArrayToQueryParam } from '../utils';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { GET_INQUIRIES_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { RequestFilterType } from '@/pages/Requests/components/ListTab/types';
import { ServerResponseError } from '@/queries/types';
import { AbsenceRequestStatus, UseGetInquiryResponse } from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetAllInquries = (
	top: number,
	skip: number,
	orderBy: string | undefined,
	filter: RequestFilterType,
	allDepartmentsLength: number,
	allAbsenceType: number,
	allUserPosition: number,
	options?: PatchedQueryOptions<
		UseGetInquiryResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<UseGetInquiryResponse, AxiosError<ServerResponseError>>({
		queryKey: [
			absencesQueriesKeys.getRequestsAbsences,
			skip,
			top,
			orderBy,
			filter,
			allDepartmentsLength,
		],
		queryFn: async () => {
			const {
				userId,
				notes,
				userPosition,
				inquiryTemplateName,
				status,
				userDepartment,
				dateFrom,
				dateTo,
				createdAtUtc,
				userFullName,
			} = filter;

			try {
				const query = buildQuery({
					filter: {
						and: [
							userId &&
								`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
									userId
								)}', Edm.String)`,
							userFullName &&
								`contains(tolower(userFullName), tolower('${replaceSpecialCharacters(
									userFullName
								)}'))`,
							notes &&
								`contains(tolower(notes), tolower('${replaceSpecialCharacters(
									notes
								)}'))`,
							inquiryTemplateName &&
								`contains(tolower(inquiryTemplateName), tolower('${replaceSpecialCharacters(
									inquiryTemplateName
								)}'))`,
							userPosition && {
								userPosition: convertFilterArrayToQueryParam(
									userPosition,
									allUserPosition
								),
							},
							userDepartment && {
								userDepartmentId: convertFilterArrayToQueryParam(
									userDepartment,
									allDepartmentsLength
								),
							},
							status && {
								status: convertFilterArrayToQueryParam(
									status,
									Object.keys(AbsenceRequestStatus).length
								),
							},
							...convertDateFromToValueToQueryParam(dateTo, 'dateTo'),
							...convertDateFromToValueToQueryParam(dateFrom, 'dateFrom'),
							...convertDateFromToValueToQueryParam(
								createdAtUtc,
								'createdAtUtc'
							),
						],
					},
					orderBy,
					top,
					skip: skip * top,
				});

				const res = await axios.get<UseGetInquiryResponse>(
					`${GET_INQUIRIES_ENDPOINT}${query}`
				);

				return res.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		refetchOnMount: true,
		...options,
	});
