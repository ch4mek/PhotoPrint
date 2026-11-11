import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useCallback, useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { ProjectItem } from '../components/ProjectItem';
import { TotalCellLegend } from '../components/TotalCellLegend';

import { ChevronButton } from '@/components/ChevronButton';
import { usePayloadContext } from '@/context/PayloadContext';
import {
	NO_ACTIVE_ASSIGNMENTS_TITLE,
	PROJECT_SUBROW_CLASS_NAME,
	TOTAL_ROW_ID,
} from '@/pages/Payload/contants';
import {
	ProjectsCellWrapper,
	ProjectsFieldsWrapper,
	ProjectsGeneralFields,
	ProjectsGeneralFieldWrapper,
	ProjectsSubFields,
} from '@/pages/Payload/styles';
import { PayloadGridRow } from '@/pages/Payload/types';
import { getNewCellHeight } from '@/pages/Payload/utils/getNewCellHeight';
import { getProjectsWithActiveAssigment } from '@/pages/Payload/utils/getProjectsWithActiveAssigment';

export const ProjectCellRenderer = (props: RenderCellProps<PayloadGridRow>) => {
	const { row, tabIndex, onRowChange } = props;
	const { expanded, projects, id } = row;

	const { showTime } = usePayloadContext();

	if (id === TOTAL_ROW_ID) {
		return <TotalCellLegend />;
	}

	const projectWithActiveAssigment = useMemo(
		() => getProjectsWithActiveAssigment(projects),
		[projects]
	);

	const updateRow = useCallback(
		(newRowParts: Partial<PayloadGridRow>) => {
			const height = getNewCellHeight(
				newRowParts.expanded!,
				projects.length,
				projectWithActiveAssigment.length
			);
			onRowChange({
				...row,
				...newRowParts,
				projectItemsRowHeight: height,
			});
		},
		[onRowChange, row, projects.length]
	);

	const handleExpandedCell = useCallback(() => {
		updateRow({ expanded: !expanded });
	}, [expanded, updateRow]);

	const height = expanded
		? getNewCellHeight(
				false,
				projects.length,
				projectWithActiveAssigment.length,
				showTime
		  )
		: undefined;

	const hasActiveAssignments = projectWithActiveAssigment.length > 0;

	const firstRowContent = hasActiveAssignments ? (
		<ProjectItem project={projectWithActiveAssigment[0]} direction="row" />
	) : (
		<Typography variant="m-400">{NO_ACTIVE_ASSIGNMENTS_TITLE}</Typography>
	);

	const hasProjects = projects.length > 0 && showTime;

	return (
		<ProjectsCellWrapper disabled={row.disabled} data-expanded={expanded}>
			<ProjectsFieldsWrapper>
				<ProjectsGeneralFields newHeight={height}>
					<ProjectsGeneralFieldWrapper isFirstRow>
						{firstRowContent}
						{hasProjects && (
							<ChevronButton
								tabIndex={tabIndex}
								onClick={handleExpandedCell}
								isActive={expanded}
							/>
						)}
					</ProjectsGeneralFieldWrapper>
					{hasActiveAssignments &&
						projectWithActiveAssigment.slice(1).map((project) => (
							<ProjectsGeneralFieldWrapper key={project.projectId}>
								<ProjectItem {...{ project }} direction="row" />
							</ProjectsGeneralFieldWrapper>
						))}
				</ProjectsGeneralFields>
				{expanded &&
					projects.map((project) => (
						<ProjectsSubFields
							key={project.projectId}
							data-subrowhoverindex={`${id}-${project.projectId}`}
							className={PROJECT_SUBROW_CLASS_NAME}
							{...{ showTime }}
						>
							<ProjectItem
								{...{ project }}
								direction={showTime ? 'column' : 'row'}
							/>
						</ProjectsSubFields>
					))}
			</ProjectsFieldsWrapper>
		</ProjectsCellWrapper>
	);
};
