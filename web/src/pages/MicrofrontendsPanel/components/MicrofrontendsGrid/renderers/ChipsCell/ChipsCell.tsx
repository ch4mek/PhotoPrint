import { useTheme } from '@emotion/react';
import { Chip } from '@mms/mms-ui-library/components/Chip';
import { useMemo } from 'react';

import { ChipsCellWrapper } from './styles';
import { ChipsCellProps } from './types';

export function ChipsCell({ row, field }: ChipsCellProps) {
	const theme = useTheme();

	const chips = useMemo(() => row[field], [row, field]);

	if (!chips || !chips.length) {
		return null;
	}

	return (
		<ChipsCellWrapper>
			{chips.map((chip, idx) => (
				<Chip
					key={idx}
					label={chip}
					withCrossIcon={false}
					wrapperStyle={{ padding: theme.spaces.xs }}
				/>
			))}
		</ChipsCellWrapper>
	);
}
