import {
	FixedTypography,
	ItemWrapper,
	NameOfFiledTypography,
} from '@/components/InfoDataContainer/Item/styles';

import { InternshipFields } from '../types';

import { TechnologiesList } from './styles';
import { TechnologyRow } from './TechnologyRow';
import { TechnologiesFieldProps } from './types';

export function TechnologiesField({ technologies }: TechnologiesFieldProps) {
	const hasTechnologies = Boolean(technologies?.length);

	return (
		<ItemWrapper>
			<NameOfFiledTypography variant="s-400">
				{InternshipFields.TECHNOLOGIES}
			</NameOfFiledTypography>
			{hasTechnologies ? (
				<TechnologiesList>
					{technologies?.map((technology) => (
						<TechnologyRow key={technology.id} technology={technology} />
					))}
				</TechnologiesList>
			) : (
				<FixedTypography variant="m-400">-</FixedTypography>
			)}
		</ItemWrapper>
	);
}
