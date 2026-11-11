import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import numeral from 'numeral';
import buildQuery from 'odata-query';

import { convertFilterArrayToQueryParam } from '../utils';

import { EquipmentQueriesKeys } from '@/constants/queriesKeys';
import { GET_EQUIPMENT_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { getStartOfDayUTC, getEndOfDayUTC } from '@/helpers/dateRangeHelpers';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { STORAGE_OPTIONS } from '@/pages/Equipment/constants';
import { EquipmentFilters } from '@/pages/Equipment/types';
import { GetEquipmentsResponse, ServerResponseError } from '@/queries/types';
import {
	DepartmentType,
	EquipmentAvailabilityType,
	EquipmentLocationType,
	EquipmentType,
} from '@/types/configuration';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetEquipments = (
	skip: number,
	top: number,
	order: SortDirection,
	orderByCondition: string,
	filter: EquipmentFilters,
	allDepartments: Array<DepartmentType>,
	allLocations: Array<EquipmentLocationType>,
	allEquipmentTypes: Array<EquipmentType>,
	allAvailabilityTypes: Array<EquipmentAvailabilityType>,
	options?: PatchedQueryOptions<
		GetEquipmentsResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<GetEquipmentsResponse, AxiosError<ServerResponseError>>({
		queryKey: [
			EquipmentQueriesKeys.equipments,
			skip,
			top,
			order,
			orderByCondition,
			filter,
		],
		queryFn: async () => {
			const {
				approver,
				availability,
				departments,
				equipmentCharacteristics,
				equipmentTypeId,
				guaranteeDate,
				id,
				location,
				name,
				notes,
				price,
				purchaseDate,
				purchasePlace,
				serialNumber,
				inventoryNumber,
				invoiceNumber,
				user,
				storage,
				repairPlace,
			} = filter;

			try {
				const searchFilter = {
					and: [
						user &&
							`contains(tolower(ActiveHolderFullName), tolower('${replaceSpecialCharacters(
								user
							)}'))`,
						equipmentCharacteristics &&
							`contains(tolower(characteristics), tolower('${replaceSpecialCharacters(
								equipmentCharacteristics
							)}'))`,
						approver &&
							`contains(tolower(ApproverFullName), tolower('${replaceSpecialCharacters(
								approver
							)}'))`,
						name &&
							`contains(tolower(name), tolower('${replaceSpecialCharacters(
								name
							)}'))`,
						serialNumber &&
							`contains(tolower(serialNumber), tolower('${replaceSpecialCharacters(
								serialNumber
							)}'))`,
						inventoryNumber &&
							`contains(tolower(inventoryNumber), tolower('${replaceSpecialCharacters(
								inventoryNumber
							)}'))`,
						invoiceNumber &&
							`contains(tolower(invoiceNumber), tolower('${replaceSpecialCharacters(
								invoiceNumber
							)}'))`,
						repairPlace &&
							`contains(tolower(repairPlace), tolower('${replaceSpecialCharacters(
								repairPlace
							)}'))`,
						purchasePlace &&
							`contains(tolower(purchasePlace), tolower('${replaceSpecialCharacters(
								purchasePlace
							)}'))`,
						notes &&
							`contains(tolower(comment), tolower('${replaceSpecialCharacters(
								notes
							)}'))`,
						id &&
							`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
								id
							)}', Edm.String)`,
						price?.from &&
							`purchasePriceUsd ge ${numeral(price?.from).value()}`,
						price?.to && `purchasePriceUsd le ${numeral(price?.to).value()}`,
						purchaseDate?.from &&
							`cast(purchaseDate, Edm.DateTimeOffset) ge cast(${getStartOfDayUTC(
								purchaseDate?.from
							)}, Edm.DateTimeOffset)`,
						purchaseDate?.to &&
							`cast(purchaseDate, Edm.DateTimeOffset) le cast(${getEndOfDayUTC(
								purchaseDate?.to
							)}, Edm.DateTimeOffset)`,

						guaranteeDate?.from &&
							`cast(guaranteeDate, Edm.DateTimeOffset) ge cast(${getStartOfDayUTC(
								guaranteeDate?.from
							)}, Edm.DateTimeOffset)`,
						guaranteeDate?.to &&
							`cast(guaranteeDate, Edm.DateTimeOffset) le cast(${getEndOfDayUTC(
								guaranteeDate?.to
							)}, Edm.DateTimeOffset)`,
					],
				};

				const query = buildQuery({
					filter: {
						and: [
							{
								status: convertFilterArrayToQueryParam(
									availability,
									allAvailabilityTypes.length
								),
							},
							{
								equipmentTypeId: convertFilterArrayToQueryParam(
									equipmentTypeId,
									allEquipmentTypes.length
								),
							},
							{
								location: convertFilterArrayToQueryParam(
									location,
									allLocations.length
								),
							},
							{
								activeHolderDepartmentId: convertFilterArrayToQueryParam(
									departments,
									allDepartments.length
								),
							},
							{
								storage: convertFilterArrayToQueryParam(
									storage,
									STORAGE_OPTIONS.length
								),
							},
							searchFilter,
						],
					},
					orderBy: `${capitalizeString(orderByCondition)} ${order}`,
					top,
					skip: skip * top,
				});

				const res = await axios.get<GetEquipmentsResponse>(
					`${GET_EQUIPMENT_API_ENDPOINT}${query}`
				);

				return res.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
