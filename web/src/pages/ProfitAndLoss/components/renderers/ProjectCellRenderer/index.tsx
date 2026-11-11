import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Fragment } from 'react';
import { RenderCellProps } from 'react-data-grid';

import {
	CellTextCutOrientation,
	TOOLTIP_FALLBACK_PLACEMENTS,
} from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';
import { PROJECT_CHILD_COLOR_CIRCLE_SIZE } from '@/pages/ProfitAndLoss/constants';
import { ProfitAndLossBaseGridRow } from '@/pages/ProfitAndLoss/types';

import { PROJECTS_LIST_SEPARATOR } from './constants';
import {
	ProjectCellContent,
	ProjectCellText,
	ProjectInlineColorCircle,
	ProjectNameSeparator,
} from './styles';

export function ProjectCellRenderer<TRow extends ProfitAndLossBaseGridRow>({
	row,
}: RenderCellProps<TRow>) {
	const { projects, disabled, isParent, id } = row;
	const isChildRow = !isParent;
	const circleSize = isChildRow ? PROJECT_CHILD_COLOR_CIRCLE_SIZE : undefined;

	const tooltipText = projects
		.map(({ name }) => name)
		.join(`${PROJECTS_LIST_SEPARATOR} `);

	if (projects.length === 0) {
		return null;
	}

	return (
		<ProjectCellContent {...{ disabled }}>
			<Tooltip
				zIndex={zIndexes.gridTooltip}
				text={tooltipText}
				position="bottom"
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				textCutOrientation={CellTextCutOrientation.Horizontal}
				useIfTextCut
			>
				<ProjectCellText {...{ isChildRow }}>
					{projects.map(({ name, color }, index) => (
						<Fragment key={`${id}-${index}`}>
							{color && <ProjectInlineColorCircle {...{ color, circleSize }} />}
							{name}
							{index < projects.length - 1 && (
								<ProjectNameSeparator>
									{PROJECTS_LIST_SEPARATOR}
								</ProjectNameSeparator>
							)}
						</Fragment>
					))}
				</ProjectCellText>
			</Tooltip>
		</ProjectCellContent>
	);
}
