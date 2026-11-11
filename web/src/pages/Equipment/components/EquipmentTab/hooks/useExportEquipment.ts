import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import numeral from 'numeral';
import buildQuery from 'odata-query';

import { STORAGE_OPTIONS } from '../../../constants';
import { EquipmentFilters } from '../../../types';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { GET_EQUIPMENT_FILE } from '@/constants/queryPaths';
import { useToast } from '@/context/Toast';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createToast } from '@/helpers/createToast';
import { getCorrectUTCStringFormatFromDate } from '@/helpers/getCorrectUTCStringFormatFromDate';
import { getEncodedQueryString } from '@/helpers/getEncodedQueryString';
import {
	AxiosResponseWithContentDisposition,
	getFileNameFromExportRespone,
} from '@/helpers/getFileNameFromExportResponse';
import { triggerFileDownload } from '@/helpers/triggerFileDownload';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { convertFilterArrayToQueryParam } from '@/queries/utils';
import {
	DepartmentType,
	EquipmentAvailabilityType,
	EquipmentLocationType,
	EquipmentType,
} from '@/types/configuration';

export const useExportEquipment = (
	format: string,
	order: string,
	orderBy: string,
	filter: EquipmentFilters,
	setFormat: React.Dispatch<React.SetStateAction<string>>,
	allDepartments: Array<DepartmentType>,
	allLocations: Array<EquipmentLocationType>,
	allEquipmentTypes: Array<EquipmentType>,
	allAvailabilityTypes: Array<EquipmentAvailabilityType>,
	top = 100000000
) => {
	const toast = useToast();

	return useQueryWithCallback({
		queryKey: ['exportEquipment', format],
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
							`contains(tolower(ActiveHolderFullName), tolower('${getEncodedQueryString(
								user
							)}'))`,
						equipmentCharacteristics &&
							`contains(tolower(characteristics), tolower('${getEncodedQueryString(
								equipmentCharacteristics
							)}'))`,
						approver &&
							`contains(tolower(ApproverFullName), tolower('${getEncodedQueryString(
								approver
							)}'))`,
						name &&
							`contains(tolower(name), tolower('${getEncodedQueryString(
								name
							)}'))`,
						repairPlace &&
							`contains(tolower(repairPlace), tolower('${getEncodedQueryString(
								repairPlace
							)}'))`,
						serialNumber &&
							`contains(tolower(serialNumber), tolower('${getEncodedQueryString(
								serialNumber
							)}'))`,
						inventoryNumber &&
							`contains(tolower(inventoryNumber), tolower('${getEncodedQueryString(
								inventoryNumber
							)}'))`,
						invoiceNumber &&
							`contains(tolower(invoiceNumber), tolower('${getEncodedQueryString(
								invoiceNumber
							)}'))`,
						purchasePlace &&
							`contains(tolower(purchasePlace), tolower('${getEncodedQueryString(
								purchasePlace
							)}'))`,
						notes &&
							`contains(tolower(comment), tolower('${getEncodedQueryString(
								notes
							)}'))`,
						id &&
							`cast(id, Edm.String) eq cast('${getEncodedQueryString(
								id
							)}', Edm.String)`,
						price?.from &&
							`purchasePriceUsd ge ${numeral(price?.from).value()}`,
						price?.to && `purchasePriceUsd le ${numeral(price?.to).value()}`,
						purchaseDate?.from &&
							`cast(purchaseDate, Edm.DateTimeOffset) ge cast(${getCorrectUTCStringFormatFromDate(
								purchaseDate?.from
							)}, Edm.DateTimeOffset)`,
						purchaseDate?.to &&
							`cast(purchaseDate, Edm.DateTimeOffset) le cast(${getCorrectUTCStringFormatFromDate(
								purchaseDate?.to
							)}, Edm.DateTimeOffset)`,

						guaranteeDate?.from &&
							`cast(guaranteeDate, Edm.DateTimeOffset) ge cast(${getCorrectUTCStringFormatFromDate(
								guaranteeDate?.from
							)}, Edm.DateTimeOffset)`,
						guaranteeDate?.to &&
							`cast(guaranteeDate, Edm.DateTimeOffset) le cast(${getCorrectUTCStringFormatFromDate(
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
					orderBy: `${capitalizeString(orderBy)} ${order}`,
					top,
					skip: 0,
				});

				const res = await axios.get(`${GET_EQUIPMENT_FILE}/${format}${query}`, {
					headers: {
						Accept:
							'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
						'Content-Disposition': 'attachment',
					},

					responseType: 'blob',
					paramsSerializer: {
						indexes: true,
					},
				});

				const fileName = getFileNameFromExportRespone(
					res as unknown as AxiosResponseWithContentDisposition
				);

				triggerFileDownload(res.data, fileName ?? `Equipment.${format}`);

				return await Promise.resolve();
			} catch (error: unknown) {
				if ((error as AxiosError)?.message) {
					toast.open(
						createToast(
							NotificationType.ERROR,
							(error as AxiosError).message || SOMETHING_WENT_WRONG
						)
					);
				}

				return await Promise.reject(error);
			} finally {
				setFormat('');
			}
		},
		enabled: format !== '',
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
};
