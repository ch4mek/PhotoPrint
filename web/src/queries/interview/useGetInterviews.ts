import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';
import { andFilter, containsFilter, orFilter } from '../utils/filters';
import { parseUserSearchString } from '../utils/parseUserSearchString';

import { interviewQueriesKeys } from '@/constants/queriesKeys';
import { GET_INTERVIEWS_SEARCH_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { InterviewFilters } from '@/pages/Staffing/components/InterviewTab/types';
import { GetInterviewDataResponse } from '@/types/interview';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetInterviews = (
	top: number,
	skip: number,
	orderBy: string | undefined,
	filter: InterviewFilters,
	options?: PatchedQueryOptions<
		GetInterviewDataResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetInterviewDataResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			interviewQueriesKeys.getInterviewsData,
			skip,
			top,
			orderBy,
			filter,
		],
		queryFn: async () => {
			const {
				id,
				specialization,
				user,
				client,
				type,
				date,
				time,
				status,
				saleManager,
				jiraTask,
				reasonForRefusal,
				note,
				userId,
			} = filter;

			const parsedUser = user?.trim()
				? parseUserSearchString(user.trim())
				: null;

			let userFilter = null;

			if (parsedUser) {
				const { russianName, englishName, fallbackTerm } = parsedUser;

				if (russianName && englishName) {
					userFilter = andFilter([
						containsFilter('userFullNameRu', russianName),
						containsFilter('userFullName', englishName),
					]);
				} else if (russianName) {
					userFilter = containsFilter('userFullNameRu', russianName);
				} else if (englishName) {
					userFilter = containsFilter('userFullName', englishName);
				} else if (fallbackTerm) {
					userFilter = orFilter([
						containsFilter('userFullName', fallbackTerm),
						containsFilter('userFullNameRu', fallbackTerm),
					]);
				}
			}

			const parsedSaleManager = saleManager?.trim()
				? parseUserSearchString(saleManager.trim())
				: null;

			let saleManagerFilter = null;

			if (parsedSaleManager) {
				const { russianName, englishName, fallbackTerm } = parsedSaleManager;

				if (russianName && englishName) {
					saleManagerFilter = andFilter([
						containsFilter('salesResponsibleUserFullNameRu', russianName),
						containsFilter('salesResponsibleUserFullName', englishName),
					]);
				} else if (russianName) {
					saleManagerFilter = containsFilter(
						'salesResponsibleUserFullNameRu',
						russianName
					);
				} else if (englishName) {
					saleManagerFilter = containsFilter(
						'salesResponsibleUserFullName',
						englishName
					);
				} else if (fallbackTerm) {
					saleManagerFilter = orFilter([
						containsFilter('salesResponsibleUserFullName', fallbackTerm),
						containsFilter('salesResponsibleUserFullNameRu', fallbackTerm),
					]);
				}
			}

			try {
				const query = buildQuery({
					filter: {
						and: [
							id?.trim() &&
								`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
									id.trim()
								)}', Edm.String)`,
							specialization?.trim() &&
								`contains(tolower(specialization), tolower('${replaceSpecialCharacters(
									specialization.trim()
								)}'))`,
							client?.trim() &&
								`contains(tolower(clientName), tolower('${replaceSpecialCharacters(
									client.trim()
								)}'))`,
							userFilter,
							type?.length && {
								interviewType: { in: type },
							},
							status?.length && {
								status: { in: status },
							},
							saleManagerFilter,
							jiraTask?.trim() &&
								`contains(tolower(jiraTask), tolower('${replaceSpecialCharacters(
									jiraTask.trim()
								)}'))`,
							reasonForRefusal?.trim() &&
								`contains(tolower(reasonForRefusal), tolower('${replaceSpecialCharacters(
									reasonForRefusal.trim()
								)}'))`,
							note?.trim() &&
								`contains(tolower(notes), tolower('${replaceSpecialCharacters(
									note.trim()
								)}'))`,
							...convertDateFromToValueToQueryParam(date, 'userInterviewDate'),
							time?.from !== null &&
								`hour(userInterviewFromUtc) ge ${Number(time.from)}`,
							time?.to !== null &&
								`hour(userInterviewFromUtc) le ${Number(time.to)}`,
							userId?.length && `userId eq ${userId}`,
						],
					},
					orderBy,
					top,
					skip: skip * top,
				});

				const res = await axios.get<GetInterviewDataResponse>(
					`${GET_INTERVIEWS_SEARCH_ENDPOINT}${query}`
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
