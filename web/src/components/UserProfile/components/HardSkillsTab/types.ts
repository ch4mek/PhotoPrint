import { GridCellDefaultProps } from '@/components/Grid';
import { FullHardSkillInfo, UserSpecialization } from '@/types/userProfile';

import { SpecializationTableFilters } from './components/SpecializationTable/types';

export enum SpecializationTableFiltersKeys {
	SpecializationName = 'name',
	Set = 'set',
	Reset = 'reset',
}

export type SpecializationAction = {
	type: SpecializationTableFiltersKeys;
	payload: string | SpecializationTableFilters;
};

export enum SpecializationRowType {
	Specialization = 'specialization',
	HardSkills = 'hardSkills',
}

export interface SpecializationRow extends GridCellDefaultProps {
	id: number;
	specializationId: number;
	hardSkills: Array<FullHardSkillInfo>;
	isPrimary: boolean;
	name: string;
	snapshots: SpecializationSnapshotsByDates;
	isExpanded: boolean;
	height: number;
	type: SpecializationRowType;
	isEven: boolean;
}

export type SpecializationSnapshotsByDates = Record<
	string,
	UserSpecialization | undefined
>;

export interface SpecializationHardSkillRow extends GridCellDefaultProps {
	id: number;
	specializationId: number;
	name: string;
	snapshots: SpecializationSnapshotsByDates;
	isActive: boolean;
}
