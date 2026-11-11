import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Column, SortColumn } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid';

export interface SoftSkillsTableProps {
	rows: SoftSkillRow[];
	columns: Column<SoftSkillRow>[];
	snapshotsDates: SoftSkillsSnapshotDates[];
	handleSortColumnChange: (newSortColumns: SortColumn[]) => void;
	sortColumns: SortColumn[];
	paginationProps?: GridPaginationProps;
}

export interface SoftSkill {
	id: number;
	softSkillName: string;
	softSkillSeniority: string;
}

export type SoftSkillSnapshotsByDates = Record<string, SoftSkill>;

export interface SoftSkillsSnapshotDates {
	snapshotId: number;
	snapshotDate: string;
}

export type SoftSkillRow = GridCellDefaultProps &
	Omit<UserSoftSkill, 'snapshots'> & {
		snapshots: SoftSkillSnapshotsByDates;
	};

interface UserSoftSkillSnapshots {
	snapshotId: number;
	snapshotDate: string;
	softSkillSeniority: string;
	softSkillSeniorityOrderWeight: number;
}

interface ShortSoftSkillInfo {
	id: number;
	softSkillName: string;
}

export interface UserSoftSkill extends ShortSoftSkillInfo {
	snapshots: UserSoftSkillSnapshots[];
}

export enum SoftSkillTableFiltersKeys {
	softSkillName = 'softSkillName',
	set = 'set',
	reset = 'reset',
}

interface SkillsFilterSetter {
	softSkillName: (name: string) => void;
	set: (allFilters: SoftSkillTableFilters) => void;
	reset: VoidFunction;
}

interface SkillFilters {
	softSkillName: string;
}

export type SoftSkillTableFilters = SkillFilters;

interface SkillAction<Tkeys, Tstate> {
	type: Tkeys;
	payload?: string[] | string | Tstate;
}

export type SoftSkillAction = SkillAction<
	SoftSkillTableFiltersKeys,
	SoftSkillTableFilters
>;

export type SoftSkillsFilterSetter = SkillsFilterSetter;
