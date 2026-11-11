import { PropsWithChildren } from 'react';

import { SelectedDesk } from '@/context/types';
import { WorkplaceInfo } from '@/queries/booking/types';
import { DeskSize } from '@/types/booking';

export enum DeskDirectionRotateDegree {
	Left = 180,
	Bottom = 90,
	Right = 0,
	Top = 270,
}

export interface Position {
	top: number;
	left: number;
}

interface RotatePosition extends Position {
	rotate: number;
}

export interface OfficeDeskPosition extends Position {
	rotationDegree: number;
	id: number;
	displayNumber: number;
	isDisabled: boolean;
	rotate?: number;
}

export enum OfficeSpaceType {
	Space = 'Space',
	MeetingRooms = 'MeetingRooms',
}

export interface SpaceConfiguration {
	type: OfficeSpaceType.Space;
	SpaceLayout: (props: PropsWithChildren & { id?: string }) => React.ReactNode;
	seats: Array<OfficeDeskPosition>;
	spaceAbbreviation: string;
	deskSize: DeskSize;
}

interface MeetingRoomsConfiguration {
	type: OfficeSpaceType.MeetingRooms;
}

export type OfficeWorkplaceConfiguration =
	| SpaceConfiguration
	| MeetingRoomsConfiguration;

export type WorkSpaceConfigurationGenerator = (
	workplacesInfo: Array<WorkplaceInfo>
) => SpaceConfiguration;

export type DeskInfo = Pick<
	SelectedDesk,
	'displayNumber' | 'id' | 'isCanCanceled'
>;

export interface SeatGenerationConfig {
	func: (
		padding: number,
		displayNumber: number,
		id: number,
		isDisabled: boolean
	) => OfficeDeskPosition;
	paddings: number[];
}

export interface DeskGenerationConfig {
	left: number;
	top: number[];
	direction: DeskDirectionRotateDegree;
}

export interface DeskSpaceEGenerationConfig {
	positions: RotatePosition[];
	direction: DeskDirectionRotateDegree;
}
