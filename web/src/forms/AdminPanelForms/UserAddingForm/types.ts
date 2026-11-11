import { Theme } from '@emotion/react';
import { UseMutateAsyncFunction } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { Dayjs } from 'dayjs';

import { CreateUserOptions, ServerResponseError } from '@/queries/types';
import {
	CalendaryCountry,
	DepartmentType,
	LocationType,
	RoleType,
} from '@/types/configuration';
import { DepartmentGroup } from '@/types/department';
import {
	GraduateJobPlacement,
	MilitaryServiceStatus,
	UserNameValues,
} from '@/types/personalData';

export interface UserAddingFormProps {
	hasHRModuleClaim: boolean;
	isLoading: boolean;
	onSubmit: UseMutateAsyncFunction<
		unknown,
		AxiosError<ServerResponseError, any>,
		CreateUserOptions,
		unknown
	>;
	onClose: VoidFunction;
}

export interface GetPopUpStylesConfigProps {
	theme: Theme;
	windowHeight: number;
	windowWidth?: number;
}

export interface AdminPanelUserAddingFormValues extends UserNameValues {
	dateOfBirth: Dayjs | null;
	militaryServiceStatus: MilitaryServiceStatus | null;

	// Contacts
	personalPhone: string | null;
	telegram: string | null;
	personalEmail: string | null;
	personalEmailSecond: string | null;
	linkedIn: string | null;

	// Work details
	position: RoleType | null;
	department: DepartmentType | null;
	group: DepartmentGroup | null;
	corporateEmail: string | null;

	// Contract details
	hireDate: Dayjs | null;
	contractStartDate: Dayjs | null;
	contractEndDate: Dayjs | null;
	graduateJobPlacement: GraduateJobPlacement | null;

	// Location
	currentLocation: string | null;
	officeLocation: LocationType | null;
	registeredLocation: CalendaryCountry | null;
}
