import { PropsWithChildren } from 'react';

import {
	MEETING_ROOMS_TAB_ID,
	MINSK_SPACE_ABBREVIATIONS,
	WARSAW_SPACE_ABBREVIATION,
} from '@/constants/booking';
import { WorkplaceInfo } from '@/queries/booking/types';
import { DeskSize } from '@/types/booking';
import {
	LocationTypeId,
	OfficeWorkspaceType as WorkspaceType,
} from '@/types/configuration';

import { MinskSpaceALayout } from './components/MinskSpaceALayout';
import { MinskSpaceBLayout } from './components/MinskSpaceBLayout';
import { MinskSpaceCLayout } from './components/MinskSpaceCLayout';
import { MinskSpaceDLayout } from './components/MinskSpaceDLayout';
import { MinskSpaceELayout } from './components/MinskSpaceELayout';
import { MinskSpaceFLayout } from './components/MinskSpaceFLayout';
import { WarsawSpaceLayout } from './components/WarsawSpaceLayout';
import {
	DeskDirectionRotateDegree,
	DeskGenerationConfig,
	DeskSpaceEGenerationConfig,
	OfficeDeskPosition,
	OfficeSpaceType,
	SeatGenerationConfig,
	SpaceConfiguration,
	WorkSpaceConfigurationGenerator,
} from './types';

const generateSeat = (
	left: number,
	top: number,
	rotationDegree: number,
	displayNumber: number,
	id: number,
	isDisabled: boolean,
	rotate?: number
): OfficeDeskPosition => ({
	id,
	left,
	top,
	displayNumber,
	rotationDegree,
	isDisabled,
	rotate,
});

class SpaceGenerator {
	seats: Array<OfficeDeskPosition> = [];

	workplacesInfo: Array<WorkplaceInfo> = [];

	abbreviation: string;

	deskSize: DeskSize;

	constructor(
		workplacesInfo: Array<WorkplaceInfo>,
		abbreviation: string,
		deskSize: DeskSize
	) {
		this.seats = [];
		this.workplacesInfo = workplacesInfo;
		this.abbreviation = abbreviation;
		this.deskSize = deskSize;
	}

	generateSeats(
		configs: Array<SeatGenerationConfig>,
		layout: (props: PropsWithChildren) => React.ReactNode
	): SpaceConfiguration {
		const { seats, workplacesInfo, abbreviation, deskSize } = this;
		configs.forEach(({ func, paddings }) => {
			paddings.forEach((padding) => {
				const index = seats.length;

				if (index >= workplacesInfo.length) {
					return;
				}

				const { id, workplaceNumber, isDisabled } = workplacesInfo[index];

				seats.push(func(padding, workplaceNumber, id, isDisabled));
			});
		});

		return {
			type: OfficeSpaceType.Space,
			SpaceLayout: layout,
			seats,
			spaceAbbreviation: abbreviation,
			deskSize,
		};
	}
}

const seatHorizontalPairsAlternation = (displayNumber: number) => {
	if (displayNumber % 2 === 0) {
		return DeskDirectionRotateDegree.Left;
	}

	return DeskDirectionRotateDegree.Right;
};

const seatVerticalPairsAlternation = (displayNumber: number) => {
	if (displayNumber % 2 === 0) {
		return DeskDirectionRotateDegree.Top;
	}

	return DeskDirectionRotateDegree.Bottom;
};

const spaceAColumnsLeftPaddings = {
	1: 1306,
	2: 1192,
	3: 1145,
	4: 1024,
	5: 972,
	6: 853,
	7: 801,
	8: 672,
	9: 620,
	10: 492,
	11: 440,
	12: 319,
	13: 267,
	14: 146,
	15: 94,
};

const spaceATopPaddings = [374, 304, 234, 164];
const createSpaceAColumnGenerator =
	(left: number, rotationDegree: number) =>
	(top: number, displayNumber: number, id: number, isDisabled: boolean) =>
		generateSeat(left, top, rotationDegree, displayNumber, id, isDisabled);

export const generateMinskSpaceA: WorkSpaceConfigurationGenerator = (
	workplacesInfo
) => {
	const minskSpaceAGenerator = new SpaceGenerator(
		workplacesInfo,
		MINSK_SPACE_ABBREVIATIONS[1],
		DeskSize.Small
	);

	return minskSpaceAGenerator.generateSeats(
		[
			{
				paddings: spaceATopPaddings.slice(0, 2),
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[1],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: spaceATopPaddings.slice(0, 3),
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[3],
					DeskDirectionRotateDegree.Right
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[4],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[5],
					DeskDirectionRotateDegree.Right
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[6],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[7],
					DeskDirectionRotateDegree.Right
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[8],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[9],
					DeskDirectionRotateDegree.Right
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[10],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[11],
					DeskDirectionRotateDegree.Right
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[12],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[13],
					DeskDirectionRotateDegree.Right
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[14],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: spaceATopPaddings,
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[15],
					DeskDirectionRotateDegree.Right
				),
			},
			{
				paddings: [5],
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[1],
					DeskDirectionRotateDegree.Left
				),
			},
			{
				paddings: [5],
				func: createSpaceAColumnGenerator(
					spaceAColumnsLeftPaddings[2],
					DeskDirectionRotateDegree.Left
				),
			},
		],
		MinskSpaceALayout
	);
};

const spaceBConfig: Array<DeskGenerationConfig> = [
	{
		left: 50,
		top: [103, 173, 243, 313, 383, 453],
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 210,
		top: [52, 122, 192, 262, 332],
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 262,
		top: [52, 122, 192, 262, 332],

		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 422,
		top: [83, 153, 223, 293, 363, 433],
		direction: DeskDirectionRotateDegree.Right,
	},
];

const minskSpaceBColumnGenerator =
	(left: number, rotationDegree: DeskDirectionRotateDegree) =>
	(top: number, displayNumber: number, id: number, isDisabled: boolean) =>
		generateSeat(left, top, rotationDegree, displayNumber, id, isDisabled);

export const generateMinskSpaceB: WorkSpaceConfigurationGenerator = (
	workplacesInfo
) => {
	const minskSpaceBGenerator = new SpaceGenerator(
		workplacesInfo,
		MINSK_SPACE_ABBREVIATIONS[2],
		DeskSize.Small
	);

	return minskSpaceBGenerator.generateSeats(
		spaceBConfig.map(({ top, left, direction }) => ({
			paddings: top,
			func: minskSpaceBColumnGenerator(left, direction),
		})),
		MinskSpaceBLayout
	);
};

const spaceCConfig: Array<DeskGenerationConfig> = [
	{
		left: 8,
		top: [114, 184, 254, 324, 394],
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 211,
		top: [148, 218, 288, 358],

		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 264,
		top: [148, 218, 288, 358],
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 467,
		top: [182, 252, 322, 392, 462],
		direction: DeskDirectionRotateDegree.Right,
	},
];

const minskSpaceCColumnGenerator =
	(left: number, rotationDegree: DeskDirectionRotateDegree) =>
	(top: number, displayNumber: number, id: number, isDisabled: boolean) =>
		generateSeat(left, top, rotationDegree, displayNumber, id, isDisabled);

export const generateMinskSpaceC: WorkSpaceConfigurationGenerator = (
	workplacesInfo
) => {
	const minskSpaceCGenerator = new SpaceGenerator(
		workplacesInfo,
		MINSK_SPACE_ABBREVIATIONS[3],
		DeskSize.Small
	);

	return minskSpaceCGenerator.generateSeats(
		spaceCConfig.map(({ top, left, direction }) => ({
			paddings: top,
			func: minskSpaceCColumnGenerator(left, direction),
		})),
		MinskSpaceCLayout
	);
};

const spaceDConfig: Array<DeskGenerationConfig> = [
	{
		left: 9,
		top: [66, 136, 206, 276],
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 187,
		top: [65, 135, 205],

		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 239,
		top: [65, 135, 205],
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 416,
		top: [37, 107, 177, 247, 317],
		direction: DeskDirectionRotateDegree.Right,
	},
];

const minskSpaceDColumnGenerator =
	(left: number, rotationDegree: DeskDirectionRotateDegree) =>
	(top: number, displayNumber: number, id: number, isDisabled: boolean) =>
		generateSeat(left, top, rotationDegree, displayNumber, id, isDisabled);

export const generateMinskSpaceD: WorkSpaceConfigurationGenerator = (
	workplacesInfo
) => {
	const minskSpaceDGenerator = new SpaceGenerator(
		workplacesInfo,
		MINSK_SPACE_ABBREVIATIONS[4],
		DeskSize.Small
	);

	return minskSpaceDGenerator.generateSeats(
		spaceDConfig.map(({ top, left, direction }) => ({
			paddings: top,
			func: minskSpaceDColumnGenerator(left, direction),
		})),
		MinskSpaceDLayout
	);
};

const spaceEConfig: Array<DeskSpaceEGenerationConfig> = [
	{
		positions: [
			{ left: 15, top: 43, rotate: -4 },
			{ left: 20, top: 116, rotate: -4 },
			{ left: 25, top: 189, rotate: -4 },
			{ left: 30, top: 262, rotate: -4 },
		],
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		positions: [
			{ left: 177, top: 33, rotate: 6 },
			{ left: 170, top: 104, rotate: 6 },
		],
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		positions: [
			{ left: 230, top: 37, rotate: 6 },
			{ left: 223, top: 109, rotate: 6 },
		],
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		positions: [
			{ left: 353, top: 37, rotate: 6 },
			{ left: 345, top: 108, rotate: 6 },
			{ left: 333, top: 207, rotate: 6 },
			{ left: 326, top: 279, rotate: 6 },
		],
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		positions: [
			{ left: 406, top: 42, rotate: 6 },
			{ left: 398, top: 113, rotate: 6 },
			{ left: 386, top: 212, rotate: 6 },
		],
		direction: DeskDirectionRotateDegree.Left,
	},
];

const minskSpaceEColumnGenerator =
	(left: number, rotationDegree: DeskDirectionRotateDegree, rotate: number) =>
	(top: number, displayNumber: number, id: number, isDisabled: boolean) =>
		generateSeat(
			left,
			top,
			rotationDegree,
			displayNumber,
			id,
			isDisabled,
			rotate
		);

export const generateMinskSpaceE: WorkSpaceConfigurationGenerator = (
	workplacesInfo
) => {
	const minskSpaceEGenerator = new SpaceGenerator(
		workplacesInfo,
		MINSK_SPACE_ABBREVIATIONS[5],
		DeskSize.Small
	);

	return minskSpaceEGenerator.generateSeats(
		spaceEConfig
			.map(({ direction, positions }) =>
				positions.map(({ top, left, rotate }) => ({
					paddings: [top],
					func: minskSpaceEColumnGenerator(left, direction, rotate),
				}))
			)
			.flat(),
		MinskSpaceELayout
	);
};

const SPACE_F_TOP_SECTION_CONFIG = [286, 216, 146];
const SPACE_F_BOTTOM_SECTION_CONFIG = [703, 633, 563, 493, 423];

const spaceFConfig: Array<DeskGenerationConfig> = [
	{
		left: 828,
		top: SPACE_F_BOTTOM_SECTION_CONFIG.slice(0, 2),
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 776,
		top: SPACE_F_BOTTOM_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 624,
		top: SPACE_F_BOTTOM_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 572,
		top: SPACE_F_BOTTOM_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 420,
		top: SPACE_F_BOTTOM_SECTION_CONFIG.slice(0, -1),
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 368,
		top: SPACE_F_BOTTOM_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 216,
		top: SPACE_F_BOTTOM_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 164,
		top: SPACE_F_BOTTOM_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 42,
		top: [617, 547, 477, 407],
		direction: DeskDirectionRotateDegree.Left,
	},

	{
		left: 42,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 164,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 216,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 368,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 420,
		top: SPACE_F_TOP_SECTION_CONFIG.slice(-2),
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 572,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 624,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Left,
	},
	{
		left: 776,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Right,
	},
	{
		left: 828,
		top: SPACE_F_TOP_SECTION_CONFIG,
		direction: DeskDirectionRotateDegree.Left,
	},
];

const minskSpaceFColumnGenerator =
	(left: number, rotationDegree: DeskDirectionRotateDegree) =>
	(top: number, displayNumber: number, id: number, isDisabled: boolean) =>
		generateSeat(left, top, rotationDegree, displayNumber, id, isDisabled);

export const generateMinskSpaceF: WorkSpaceConfigurationGenerator = (
	workplacesInfo
) => {
	const minskSpaceFGenerator = new SpaceGenerator(
		workplacesInfo,
		MINSK_SPACE_ABBREVIATIONS[9],
		DeskSize.Small
	);

	return minskSpaceFGenerator.generateSeats(
		spaceFConfig.map(({ top, left, direction }) => ({
			paddings: top,
			func: minskSpaceFColumnGenerator(left, direction),
		})),
		MinskSpaceFLayout
	);
};

const WARSAW_LAST_COLUMN_LEFT_PADDING = 330;
const warsawLastColumn = (
	top: number,
	displayNumber: number,
	id: number,
	isDisabled: boolean
) =>
	generateSeat(
		WARSAW_LAST_COLUMN_LEFT_PADDING,
		top,
		seatVerticalPairsAlternation(displayNumber),
		displayNumber,
		id,
		isDisabled
	);

const WARSAW_TOP_ROW_TOP_PADDING = 40;
const warsawTopRow = (
	left: number,
	displayNumber: number,
	id: number,
	isDisabled: boolean
) =>
	generateSeat(
		left,
		WARSAW_TOP_ROW_TOP_PADDING,
		DeskDirectionRotateDegree.Top,
		displayNumber,
		id,
		isDisabled
	);

const WARSAW_BOTTOM_ROW_TOP_PADDING = 330;
const warsawBottomRow =
	(alternationCoefficient = 1) =>
	(left: number, displayNumber: number, id: number, isDisabled: boolean) =>
		generateSeat(
			left,
			WARSAW_BOTTOM_ROW_TOP_PADDING,
			seatHorizontalPairsAlternation(displayNumber + alternationCoefficient),
			displayNumber,
			id,
			isDisabled
		);

export const generateWarsawSpace: WorkSpaceConfigurationGenerator = (
	workplacesInfo
) => {
	const warsawGenerator = new SpaceGenerator(
		workplacesInfo,
		WARSAW_SPACE_ABBREVIATION,
		DeskSize.Large
	);

	return warsawGenerator.generateSeats(
		[
			{
				paddings: [80, 150, 228],
				func: warsawLastColumn,
			},
			{
				paddings: [230],
				func: warsawBottomRow(2),
			},
			{
				paddings: [210],
				func: warsawTopRow,
			},

			{
				paddings: [160, 70, 0],
				func: warsawBottomRow(),
			},
			{
				paddings: [70],
				func: warsawTopRow,
			},
		],
		WarsawSpaceLayout
	);
};

export const getOfficeActiveTabs = (
	officeLocation: LocationTypeId,
	workspaces: Array<WorkspaceType>
) => {
	if (officeLocation === LocationTypeId.Minsk) {
		return [
			...workspaces.map(({ internalCode }) => internalCode),
			MEETING_ROOMS_TAB_ID,
		];
	}

	return [];
};
