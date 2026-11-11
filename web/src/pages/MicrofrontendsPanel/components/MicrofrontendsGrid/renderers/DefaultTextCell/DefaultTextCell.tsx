import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { DefaultTextCellText } from './styles';
import { DefaultTextCellProps } from './types';

export function DefaultTextCell({ row, field }: DefaultTextCellProps) {
	const value = row[field] as string;

	if (!value) {
		return <span>-</span>;
	}

	return (
		<Tooltip text={value} useIfTextCut>
			<DefaultTextCellText>{value}</DefaultTextCellText>
		</Tooltip>
	);
}
