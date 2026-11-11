import { useTheme } from '@emotion/react';
import { Chip } from '@mms/mms-ui-library/components/Chip';
import { RenderCellProps } from 'react-data-grid';

import { MicrofrontendsGridRow } from '../../types';

import { ClaimsCellWrapper } from './styles';

export function ClaimsCell({ row }: RenderCellProps<MicrofrontendsGridRow>) {
	const theme = useTheme();

	const { claims } = row;

	if (!claims || claims.length === 0) {
		return (
			<ClaimsCellWrapper>
				<Chip
					labelStyle={{ padding: theme.spaces.xs }}
					withCrossIcon={false}
					label="Public"
				/>
			</ClaimsCellWrapper>
		);
	}

	return (
		<ClaimsCellWrapper>
			{claims.map((claim, idx) => (
				<Chip
					key={idx}
					label={claim}
					withCrossIcon={false}
					labelStyle={{ padding: theme.spaces.xs }}
				/>
			))}
		</ClaimsCellWrapper>
	);
}
