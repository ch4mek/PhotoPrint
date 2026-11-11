import { capitalizeString } from '@/helpers/capitalizeString';

import {
	CHART_TOP_OFFSET,
	TOTAL_BAR_NAME,
	Y_AXIS_DOMAIN_MAX_BASE_SIZE,
	Y_AXIS_TICKS_STEP,
} from './constants';
import { ChartStackedProjectAdapted, KeyColorItem } from './types';

export const getHoursFromNumber = (number: number): number =>
	Math.floor(number / 60);

export const getMinutesFromNumber = (number: number): number => number % 60;

export function calculateMaxYAxisValueInHours(
	data: ChartStackedProjectAdapted[]
) {
	let maxHeight = 0;

	data.forEach((item) => {
		maxHeight = Math.max(maxHeight, item.totalTimeInMinutes);
	});

	const maxHours = maxHeight / 60;

	return Math.ceil(maxHours * (Y_AXIS_DOMAIN_MAX_BASE_SIZE + CHART_TOP_OFFSET));
}

function calcluateTotalProjectHours(
	maxYAxisValueInHours: number,
	projectTotalTimeInMinutes: number
) {
	const adjustedHours =
		maxYAxisValueInHours === Y_AXIS_DOMAIN_MAX_BASE_SIZE
			? maxYAxisValueInHours * Y_AXIS_TICKS_STEP
			: maxYAxisValueInHours;

	return adjustedHours - projectTotalTimeInMinutes / 60;
}

function addMockedTotalProject(
	data: ChartStackedProjectAdapted[],
	maxYAxisValueInHours: number | undefined,
	mockProjectColor: string
) {
	data.forEach((item) => {
		if (item.projects.at(-1)?.projectId !== TOTAL_BAR_NAME) {
			item.projects.push({
				projectId: TOTAL_BAR_NAME,
				projectName: capitalizeString(TOTAL_BAR_NAME),
				projectColor: mockProjectColor,
				isBillableProject: false,
				hours: getHoursFromNumber(item.totalTimeInMinutes),
				minutes: item.totalTimeInMinutes % 60,
				hoursForBar: calcluateTotalProjectHours(
					maxYAxisValueInHours ?? 0,
					item.totalTimeInMinutes
				),
				totalTime: item.totalTimeInMinutes,
			});
		}
	});
}

const sortProjects = (a: KeyColorItem, b: KeyColorItem) =>
	a.projectId < b.projectId ? -1 : 1;

export function createKeyWithNameAndColorMap(
	data: ChartStackedProjectAdapted[],
	maxYAxisValueInHours: number | undefined,
	mockProjectColor = 'transparent'
) {
	const billableProjectsMap = new Map<string, KeyColorItem>();
	const nonBillableProjectsMap = new Map<string, KeyColorItem>();

	data.forEach((item) =>
		item.projects.forEach(
			({ projectId, projectName, projectColor, isBillableProject }) => {
				if (
					!isBillableProject &&
					projectId !== TOTAL_BAR_NAME &&
					!nonBillableProjectsMap.has(projectId)
				) {
					nonBillableProjectsMap.set(projectId, {
						projectId,
						projectName,
						projectColor,
					});
				} else if (isBillableProject && !billableProjectsMap.has(projectId)) {
					billableProjectsMap.set(projectId, {
						projectId,
						projectName,
						projectColor,
					});
				}
			}
		)
	);

	addMockedTotalProject(data, maxYAxisValueInHours, mockProjectColor);

	const projectsArray = [
		...Array.from(billableProjectsMap.values()).sort(sortProjects),
		...Array.from(nonBillableProjectsMap.values()).sort(sortProjects),
	];
	projectsArray.push({
		projectId: TOTAL_BAR_NAME,
		projectName: capitalizeString(TOTAL_BAR_NAME),
		projectColor: mockProjectColor,
	});

	return projectsArray;
}
