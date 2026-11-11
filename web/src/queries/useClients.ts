/* eslint-disable @typescript-eslint/default-param-last */
import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { FromToProps } from '@/components/Grid';
import { ActivityStatus } from '@/constants/activityStatus';
import { ClientQuerieKeys } from '@/constants/queriesKeys';
import { GET_CLIENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ClientsResponse } from '@/types/clients';
import { CalendaryCountry } from '@/types/configuration';

import { convertFilterArrayToQueryParam } from './utils';

const paramsToReverseOrder = ['isDeleted', 'isActive'];

export const useClients = (
	page: number,
	rowsPerPage: number,
	order: SortDirection,
	orderByCondition: string,
	clientNameFilter: string,
	notesFilter: string,
	registeredLocation: string[],
	idFilter: string,
	statusFilter: string[],
	allRegisteredLocations: CalendaryCountry[],
	{ from: factFrom, to: factTo }: FromToProps<string>,
	{ from: billableFrom, to: billableTo }: FromToProps<string>
) =>
	useQueryWithCallback({
		queryKey: [
			ClientQuerieKeys.clients,
			page,
			rowsPerPage,
			order,
			orderByCondition,
			clientNameFilter,
			notesFilter,
			registeredLocation,
			factFrom,
			factTo,
			billableFrom,
			billableTo,
			idFilter,
			statusFilter,
		],
		queryFn: () => {
			const query = buildQuery({
				filter: {
					and: [
						{
							and: [
								statusFilter.length === 1 && {
									isActive: statusFilter[0] === ActivityStatus.Active,
								},
								{
									registeredLocation: convertFilterArrayToQueryParam(
										registeredLocation,
										allRegisteredLocations.length
									),
								},
								clientNameFilter &&
									`contains(tolower(Name), tolower('${replaceSpecialCharacters(
										clientNameFilter
									)}'))`,
								notesFilter &&
									`contains(tolower(Comments), tolower('${replaceSpecialCharacters(
										notesFilter
									)}'))`,
								factFrom &&
									`factTrackedTimeInMinutes ge ${replaceSpecialCharacters(
										String(+factFrom * 60)
									)}`,
								factTo &&
									`factTrackedTimeInMinutes le ${replaceSpecialCharacters(
										String(+factTo * 60)
									)}`,
								billableFrom &&
									`billableTrackedTimeInMinutes ge ${replaceSpecialCharacters(
										String(+billableFrom * 60)
									)}`,
								billableTo &&
									`billableTrackedTimeInMinutes le ${replaceSpecialCharacters(
										String(+billableTo * 60)
									)}`,
								idFilter &&
									`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
										idFilter
									)}', Edm.String)`,
							],
						},
					],
				},
				orderBy: `${capitalizeString(
					orderByCondition
				)} ${convertOrderToQueryParam(
					order,
					orderByCondition,
					paramsToReverseOrder
				)}`,
				top: rowsPerPage,
				skip: (page - 1) * rowsPerPage,
			});

			return axios
				.get<ClientsResponse>(`${GET_CLIENTS_API_ENDPOINT}/search/${query}`, {})
				.then((res) => res.data);
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
