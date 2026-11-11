import { Timezone } from '@/components/Timeline/types';
import type { Roles } from '@/constants/roles';

import { MinskSpacesInternalCodes } from './booking';

export interface PersistentConfigurationItem<
	IdT = string,
	NameT = IdT,
	DescriptionT = IdT,
	LocationT = unknown
> {
	id: IdT;
	name: NameT;
	description: DescriptionT;
	location: LocationT | null;
}

export interface SimpleConfigurableConfigurationItem<IdT = string> {
	id: IdT;
	name: string;
}

export interface ComplexConfigurableConfigurationItem {
	id: string;
	shortName: string;
	fullName: string;
}

export enum LocationTypeId {
	Minsk = 'Minsk',
	Warsaw = 'Warsaw',
	Vitebsk = 'Vitebsk',
	Tbilisi = 'Tbilisi',
}

export enum RegisteredLocationTypeId {
	Belarus = 'Belarus',
	Poland = 'Poland',
}

export enum PermissionTypeId {
	TrackBillableWorkingHours = 'TrackBillableWorkingHours',
}

export enum PermissionResourceTypeId {
	Project = 'Project',
}

export interface Configuration {
	persistentValues: PersistentValues;
	configurableValues: ConfigurableValues;
}

interface PersistentValues {
	locationTypes: Array<LocationType>;
	jsStackTypes: Array<JSStackType>;
	equipmentLocationTypes: Array<EquipmentLocationType>;
	equipmentAvailabilityTypes: Array<EquipmentAvailabilityType>;
	moneyCurrencyTypes: Array<MoneyCurrencyType>;
	permissionTypes: Array<PermissionType>;
	permissionResourceTypes: Array<PermissionResourseType>;
	equipmentBrokenStatuses: Array<EquipmentBrokenStatus>;
	fireTypes: Array<FireType>;
	educationLevelTypes: Array<EducationLevel>;
	studyFormTypes: Array<StudyForm>;
	merchItemStatuses: Array<MerchItemStatusesType>;
	absenceTypes: Array<AbsencesType>;
	activityTypes: Array<ActivityType>;
	merchTransactionTypes: Array<MerchTransactionType>;
	trackingUnitTypes: Array<TrackingUnitsType>;
	merchStorageStatuses: Array<MerchStorageStatusesType>;
	accrualTypes: Array<AccrualType>;
	transferTypes: Array<TransferType>;
}

export type LocationType = PersistentConfigurationItem;
type AccrualType = PersistentConfigurationItem;
type TransferType = PersistentConfigurationItem;
export type RoleType = PersistentConfigurationItem<Roles, string, string>;
export type JSStackType = PersistentConfigurationItem;
export type EquipmentLocationType = PersistentConfigurationItem;
export type EquipmentBrokenStatus = PersistentConfigurationItem;
export type EquipmentAvailabilityType = PersistentConfigurationItem;
export type MerchItemStatusesType = PersistentConfigurationItem;
export type MerchTransactionType = PersistentConfigurationItem;
type MerchStorageStatusesType = PersistentConfigurationItem;
type FireType = PersistentConfigurationItem;
type EducationLevel = PersistentConfigurationItem;
type StudyForm = PersistentConfigurationItem;
type MoneyCurrencyType = PersistentConfigurationItem;
type PermissionType = PersistentConfigurationItem<PermissionTypeId>;
type PermissionResourseType =
	PersistentConfigurationItem<PermissionResourceTypeId>;
type AbsencesType = PersistentConfigurationItem;
type ActivityType = PersistentConfigurationItem;
type TrackingUnitsType = PersistentConfigurationItem;

interface ConfigurableValues {
	offices: Array<OfficeType>;
	roleTypes: Array<RoleType>;
	departmentTypes: Array<DepartmentType>;
	equipmentTypes: Array<EquipmentType>;
	hardSkillGroups: Array<HardSkillGroupType>;
	seniorityLevels: Array<SeniorityLevelType>;
	generalSenioritySeniorityLevels: Array<SeniorityLevelType>;
	englishLevels: Array<EnglishLevelType>;
	merchStorages: Array<MerchStorageType>;
	merchSizes: Array<MerchSizeType>;
	softSkillSeniorityLevels: Array<SoftSkillsSeniorityLevelType>;
	countries: Array<CalendaryCountry>;
}

export type OfficeWorkspaceType =
	SimpleConfigurableConfigurationItem<number> & {
		internalCode: MinskSpacesInternalCodes;
	};

export type MeetingRoomType = SimpleConfigurableConfigurationItem<number>;

export interface OfficeType
	extends SimpleConfigurableConfigurationItem<number> {
	location: LocationTypeId;
	timeZone: Timezone;
	timeZoneDisplayName: string;
	workspaces: Array<OfficeWorkspaceType>;
	meetingRooms: Array<MeetingRoomType>;
}

export type DepartmentType = ComplexConfigurableConfigurationItem;
export type EquipmentType = ComplexConfigurableConfigurationItem;
export type MerchSizeType = SimpleConfigurableConfigurationItem;
export type MerchStorageType = SimpleConfigurableConfigurationItem<number>;
type HardSkillGroupType = SimpleConfigurableConfigurationItem;
export type SeniorityLevelType = SimpleConfigurableConfigurationItem;
export type EnglishLevelType = SimpleConfigurableConfigurationItem;
export type CalendaryCountry = SimpleConfigurableConfigurationItem<number>;
type SoftSkillsSeniorityLevelType = SimpleConfigurableConfigurationItem;
