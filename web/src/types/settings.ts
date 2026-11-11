import { SelectionModalOption } from '@/components/Grid/renderers/header/types';

import { AutoselectOption } from './common';

export interface HardSkill {
	id: number;
	name: string;
	hardSkillGroupId: string;
}

export interface HardSkillResponse {
	page: number;
	pageSize: number;
	total: number;
	items: Array<HardSkill>;
}

export interface SoftSkill {
	id: number;
	name: string;
	hardSkillGroupId: string;
}

export interface SoftSkillResponse {
	page: number;
	pageSize: number;
	total: number;
	items: HardSkill[];
}

export interface Specialization {
	id: number;
	name: string;
	canBeUpdated: boolean;
}

interface SpecializationDepartment {
	departmentId: string;
	isUserFromDepartmentAssigned: boolean;
}

export interface DetailedSpecialization
	extends Omit<Specialization, 'canBeUpdated'> {
	departmentDtos: Array<SpecializationDepartment>;
	hardSkills: Array<{ id: number; name: string }>;
}

interface SpecializationParams extends Omit<Specialization, 'canBeUpdated'> {
	departments: Array<AutoselectOption<string>>;
	hardSkills: Array<{ id: number; name: string }>;
}

export type CreateSpecializationParams = SpecializationParams;
export type EditSpecializationParams = SpecializationParams;

export interface WorkingHoursPerMonth {
	id: number;
	year: number;
	monthOfYear: number;
	countryId: number;
	hours: number;
}

interface WorkingHoursPerYear {
	countryId: number;
	year: number;
	hours: number;
}
export interface WorkingHoursResponse {
	hoursPerMonth: Array<WorkingHoursPerMonth>;
	hoursPerYear: Array<WorkingHoursPerYear>;
}

export enum WorkingHoursCalendarRowType {
	CountryRow = 'country',
	NewCountryRow = 'new-country',
	AddCountryButtonRow = 'add-country',
}

export interface CalendarMonthTime {
	id: number;
	hours: number;
	isLoading: boolean;
	hasError: boolean;
}

export interface WorkingHoursCalendarRow {
	type: WorkingHoursCalendarRowType;
	countryName: string;
	countryId: number;
	timeMap: Map<number, CalendarMonthTime>;
	error?: string;
	disabled?: boolean;
}

export type CalendarCountries = Record<number, string>;

interface UserClaimFilters {
	userClaimName: string;
	userId: string;
	departments: string[];
}

export type UserClaimTableFilters = UserClaimFilters;

interface UserClaimFilterSetter {
	userClaimName: (name: string) => void;
	userId: (id: string) => void;
	departments: (departments: string[]) => void;
	set: (allFilters: UserClaimTableFilters) => void;
	reset: VoidFunction;
}

export type UserClaimsFilterSetter = UserClaimFilterSetter;

export interface UserClaimsFilterOptions {
	departments: SelectionModalOption[];
	users: string[];
}

interface ClaimAction<Tkeys, Tstate> {
	type: Tkeys;
	payload?: string[] | string | Tstate;
}

export enum UserClaimsTableFiltersKeys {
	userClaimName = 'userClaimName',
	userId = 'userId',
	departments = 'departments',
	set = 'set',
	reset = 'reset',
}

export type UserClaimAction = ClaimAction<
	UserClaimsTableFiltersKeys,
	UserClaimTableFilters
>;

interface AvailableUserClaim {
	claimType: string;
	claimValue?: string | null;
	allowedValueType?: string;
}
export interface AvailableUserClaimResponse {
	name: string;
	claims: AvailableUserClaim[];
}
