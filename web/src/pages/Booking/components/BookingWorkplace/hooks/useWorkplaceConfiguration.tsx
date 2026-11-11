import { useMemo } from 'react';

import { MINSK_SPACES, WARSAW_SPACE } from '../constants';
import { OfficeWorkplaceConfiguration, OfficeSpaceType } from '../types';
import { getOfficeActiveTabs } from '../utils';

import { useAuth } from '@/context/AuthContext';
import { areMeetingRoomsOpen } from '@/helpers/bookingTime';
import { WorkplaceInfo } from '@/queries/booking/types';
import { LocationTypeId, OfficeType } from '@/types/configuration';

const NO_SPACES_INDEX = 0;

interface UseWorkplaceConfigurationOptions {
	office: OfficeType;
	spaceId: number | null;
	officeWorkplaces: WorkplaceInfo[];
}

interface WorkplaceConfiguration {
	configuration: OfficeWorkplaceConfiguration | null;
	activeTabsId: (number | string)[];
}

export const useWorkplaceConfiguration = ({
	office,
	spaceId,
	officeWorkplaces,
}: UseWorkplaceConfigurationOptions): WorkplaceConfiguration => {
	const { departmentId, role } = useAuth();

	const spaceInternalCode = useMemo(
		() => office.workspaces.find(({ id }) => id === spaceId)?.internalCode,
		[spaceId, office.workspaces]
	);

	const spaceWorkplacesMap = useMemo(() => {
		if (!officeWorkplaces) {
			return null;
		}

		const groupedOfficeWorkplaces = officeWorkplaces.reduce<
			Record<string, WorkplaceInfo[]>
		>((acc, workplace) => {
			const { workspaceId } = workplace;

			const workspacesId = workspaceId || NO_SPACES_INDEX;

			if (!acc[workspacesId]) {
				acc[workspacesId] = [];
			}

			acc[workspacesId].push(workplace);

			return acc;
		}, {});

		Object.entries(groupedOfficeWorkplaces).forEach(([key, workspaces]) => {
			groupedOfficeWorkplaces[key] = workspaces.sort(
				(a, b) => a.workplaceNumber - b.workplaceNumber
			);
		});

		return groupedOfficeWorkplaces;
	}, [officeWorkplaces]);

	const workspaceConfiguration: WorkplaceConfiguration = useMemo(() => {
		const activeTabsId = getOfficeActiveTabs(
			office.location,
			office.workspaces
		);

		if (areMeetingRoomsOpen(spaceId)) {
			return {
				configuration: {
					type: OfficeSpaceType.MeetingRooms,
				},
				isAvailable: true,
				activeTabsId,
			};
		}

		if (
			office.location === LocationTypeId.Minsk &&
			spaceId &&
			spaceInternalCode &&
			spaceWorkplacesMap &&
			spaceWorkplacesMap[spaceId] &&
			MINSK_SPACES[spaceInternalCode]
		) {
			return {
				configuration: MINSK_SPACES[spaceInternalCode](
					spaceWorkplacesMap[spaceId]
				),
				activeTabsId,
			};
		}

		if (
			office.location === LocationTypeId.Warsaw &&
			spaceWorkplacesMap &&
			spaceWorkplacesMap[NO_SPACES_INDEX]
		) {
			return {
				configuration: WARSAW_SPACE(spaceWorkplacesMap[NO_SPACES_INDEX]),
				activeTabsId,
			};
		}

		return {
			configuration: null,
			isAvailable: true,
			activeTabsId,
		};
	}, [
		office,
		spaceId,
		spaceWorkplacesMap,
		spaceInternalCode,
		role,
		departmentId,
	]);

	return workspaceConfiguration;
};
