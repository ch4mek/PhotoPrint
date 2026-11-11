import { AdminPanelUserAddingFormValues } from '../types';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { graduateJobPlacementToBool } from '@/helpers/formatGraduateJobPlacement';
import { getDateFormat } from '@/helpers/getDateFormat';
import { CreateUserOptions } from '@/queries/types';

export const convertDataToSubmit = ({
	firstName,
	lastName,
	firstNameRu,
	lastNameRu,
	patronymicRu,
	dateOfBirth,
	militaryServiceStatus,
	personalPhone,
	telegram,
	personalEmail,
	personalEmailSecond,
	linkedIn,
	corporateEmail,
	position,
	department,
	group,
	hireDate,
	contractStartDate,
	contractEndDate,
	graduateJobPlacement,
	currentLocation,
	officeLocation,
	registeredLocation,
}: AdminPanelUserAddingFormValues): Omit<CreateUserOptions, 'countryId'> => ({
	firstName,
	lastName,
	firstNameRu: firstNameRu || null,
	lastNameRu: lastNameRu || null,
	patronymicRu: patronymicRu || null,
	dateOfBirth: getDateFormat(dateOfBirth, YYYY_MM_DD_DAYJS_DATE_FORMAT),
	militaryServiceStatus,
	personalPhone,
	telegram,
	personalEmail,
	personalEmailSecond,
	linkedIn: linkedIn || null,
	email: corporateEmail || null,
	position: position?.id ?? '',
	departmentId: department?.id ?? null,
	resourceGroupId: group?.id ?? null,
	hireDate: getDateFormat(hireDate, YYYY_MM_DD_DAYJS_DATE_FORMAT),
	contractDateFrom:
		getDateFormat(contractStartDate, YYYY_MM_DD_DAYJS_DATE_FORMAT) ?? null,
	contractDateTo:
		getDateFormat(contractEndDate, YYYY_MM_DD_DAYJS_DATE_FORMAT) ?? null,
	isGraduateJobPlacement: graduateJobPlacementToBool(graduateJobPlacement),
	currentLocation: currentLocation || null,
	location: officeLocation?.id ?? '',
	registeredLocationId: registeredLocation?.id ?? 0,
});
