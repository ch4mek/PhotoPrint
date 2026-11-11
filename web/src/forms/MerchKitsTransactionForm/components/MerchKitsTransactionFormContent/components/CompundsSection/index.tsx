import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { COMPOUNDS_TITLE } from './constants';
import {
	CompoundItem,
	CompoundsList,
	CompoundsTitle,
	CompoundsWrapper,
	ItemNameTypography,
} from './styles';
import { CompoundsSectionProps } from './types';

export function CompoundsSection({ compounds }: CompoundsSectionProps) {
	return (
		<CompoundsWrapper>
			<CompoundsTitle>{COMPOUNDS_TITLE}</CompoundsTitle>
			<CompoundsList>
				{compounds.map(({ name, amountInKit }) => (
					<CompoundItem key={name}>
						<Tooltip text={name} useIfTextCut>
							<ItemNameTypography variant="m-400">{name}</ItemNameTypography>
						</Tooltip>
						<Typography variant="m-400">x{amountInKit}</Typography>
					</CompoundItem>
				))}
			</CompoundsList>
		</CompoundsWrapper>
	);
}
