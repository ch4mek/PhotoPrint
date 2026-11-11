import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { MIN_AMOUT_EQUIPMENT } from '../../constants';

import { EquipmentItem } from './styles';
import { SectionEquipmentItemProps } from './types';

export function SectionEquipmentItem({
	name,
	icon: Icon,
	count,
}: SectionEquipmentItemProps) {
	return (
		<Tooltip text={name} open={!!name} position="bottom">
			<EquipmentItem>
				<Icon />
				{count > MIN_AMOUT_EQUIPMENT && (
					<Typography Component="span" variant="xs-400">
						x{count}
					</Typography>
				)}
			</EquipmentItem>
		</Tooltip>
	);
}
