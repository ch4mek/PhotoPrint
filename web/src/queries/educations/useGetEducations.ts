import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';
import { convertFilterArrayToQueryParam } from '../utils';

import { EducationsQueriesKeys } from '@/constants/queriesKeys';
import { getUserEducationsEndpoint } from '@/constants/queryPaths';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { EducationsResponse } from '@/types/educations';

import { UseGetEducationsProps } from './types';

const orderWeighedFields = ['educationLevel', 'studyForm'];
const ORDER_WEIGHT_POSTFIX = 'OrderWeight';

export const useGetEducations = ({
	filters,
	options = {},
	order,
	orderBy,
	page,
	rowsPerPage,
	userId,
	allFilterOptions,
}: UseGetEducationsProps) =>
	useQueryWithCallback<
		ByPageResponse<EducationsResponse>,
		AxiosError<ServerResponseError>
	>({
		...options,
		queryKey: [
			EducationsQueriesKeys.userEducations,
			userId,
			`${page}`,
			`${rowsPerPage}`,
			order,
			orderBy,
			JSON.stringify(filters),
		],
		queryFn: async () => {
			const {
				educationLevel,
				educationalInstitution,
				faculty,
				specialization,
				studyForm,
				graduationYear,
				comment,
			} = filters;
			const { educationLevels, studyForms } = allFilterOptions;

			const actualOrderBy = orderWeighedFields.includes(orderBy)
				? `${orderBy}${ORDER_WEIGHT_POSTFIX}`
				: orderBy;

			const convertedOrder = convertOrderToQueryParam(order, actualOrderBy, []);

			const query = buildQuery({
				filter: {
					and: [
						{
							educationLevel: convertFilterArrayToQueryParam(
								educationLevel,
								educationLevels.length
							),
						},
						educationalInstitution
							? `contains(tolower(educationalInstitution), tolower('${replaceSpecialCharacters(
									educationalInstitution
							  )}'))`
							: undefined,
						faculty
							? `contains(tolower(faculty), tolower('${replaceSpecialCharacters(
									faculty
							  )}'))`
							: undefined,
						specialization
							? `contains(tolower(specialization), tolower('${replaceSpecialCharacters(
									specialization
							  )}'))`
							: undefined,
						graduationYear &&
							`cast(graduationYear, Edm.String) eq cast('${replaceSpecialCharacters(
								graduationYear
							)}', Edm.String)`,
						comment
							? `contains(tolower(comment), tolower('${replaceSpecialCharacters(
									comment
							  )}'))`
							: undefined,
						{
							studyForm: convertFilterArrayToQueryParam(
								studyForm,
								studyForms.length
							),
						},
					],
				},
				orderBy: `${actualOrderBy} ${convertedOrder}`,
				top: rowsPerPage,
				skip: page * rowsPerPage,
			});

			const { data } = await axios.get(
				`${getUserEducationsEndpoint(userId)}${query}`
			);

			return data;
		},
		refetchOnWindowFocus: false,
	});
