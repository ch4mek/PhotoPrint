import { Photo } from '@mms/mms-ui-library/components/Photo';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { TEMP_AVATAR_IMG } from '../constants';
import { NameCellWrapper } from '../styles';
import { VacationCalendarCellProps } from '../types';

export const nameCellRenderer = <T extends VacationCalendarCellProps>({
	row,
	column,
}: RenderCellProps<T, unknown>) => {
	if (row.showSkeleton) {
		return <Skeleton animation="wave" width="100%" />;
	}

	const name = String(row[column.key as keyof T] ?? '');
	const position = row.position;

	if (!name) {
		return <NameCellWrapper variant="s-400" />;
	}

	return (
		<Tooltip useIfTextCut text={`${name} ${position ? `(${position})` : ''}`}>
			<NameCellWrapper variant="s-400">
				<Photo src={TEMP_AVATAR_IMG} firstName="A" lastName="b" size={30} />
				<div>
					{name}
					{position && (
						<>
							<br />
							<Typography variant="s-400" color="grey">
								{position}
							</Typography>
						</>
					)}
				</div>
			</NameCellWrapper>
		</Tooltip>
	);
};
