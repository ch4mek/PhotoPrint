import { memo } from 'react';

import { DecorationPicture } from './components/DecorationPicture';
import { DecorationsWrapper, DecorationZone } from './styles';
import { StaticDecorationsConfig } from './types';

function StaticDecorationsInner({
	placement = 'bottom',
	zonesGap,
	zones,
}: StaticDecorationsConfig) {
	if (!zones?.length) {
		return null;
	}

	return (
		<DecorationsWrapper {...{ placement, zonesGap }}>
			{zones.map((zone) => {
				const { position, styles, ...zoneProps } = zone;

				return (
					<DecorationZone
						key={position}
						position={position}
						customStyles={styles}
						{...zoneProps}
					>
						{zone.items.map(({ id, ...decorationProps }) => (
							<DecorationPicture key={id} {...decorationProps} />
						))}
					</DecorationZone>
				);
			})}
		</DecorationsWrapper>
	);
}

export const StaticDecorations = memo(StaticDecorationsInner);
