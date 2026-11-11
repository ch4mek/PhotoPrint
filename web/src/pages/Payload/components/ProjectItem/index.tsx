import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { EmployeeCoinIcon } from '../../styles';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';

import {
	ColorCircleWrapper,
	ProjectItemClientNameField,
	ProjectItemProjectNameField,
	ProjectsField,
} from './styles';
import { ProjectItemProps } from './types';

export function ProjectItem({ project, direction }: ProjectItemProps) {
	const {
		color,
		clientName,
		name,
		isTimeOnProjectBillable,
		isActiveAssigment,
		isFutureActiveAssignment,
	} = project;

	const shouldShowCoin =
		isTimeOnProjectBillable && (isActiveAssigment || isFutureActiveAssignment);

	return (
		<>
			<ProjectsField {...{ direction }}>
				<ColorCircleWrapper {...{ color }} />
				<Tooltip
					text={clientName}
					useIfTextCut
					fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				>
					<ProjectItemClientNameField variant="m-400">
						{clientName}
					</ProjectItemClientNameField>
				</Tooltip>
				<Tooltip
					text={name}
					useIfTextCut
					fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				>
					<ProjectItemProjectNameField variant="s-400" {...{ direction }}>
						{name}
					</ProjectItemProjectNameField>
				</Tooltip>
			</ProjectsField>
			{shouldShowCoin && <EmployeeCoinIcon />}
		</>
	);
}
