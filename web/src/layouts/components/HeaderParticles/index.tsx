import Particles from '@tsparticles/react';
import { memo, useMemo } from 'react';

import { useResize } from '@/hooks/useResize';

import { DecorativeIcon } from './components/DecorativeIcon';
import { StaticDecorations } from './components/StaticDecorations';
import { useParticlesInit } from './hooks/useParticlesInit';
import { ParticlesWrapper } from './styles';
import { HeaderDecorationsProps } from './types';
import { getParticlesOptions } from './utils/getParticlesOptions';

function HeaderDecorationsInner({
	decorativeIcon,
	anchorRef,
	particles,
	staticDecorations,
}: HeaderDecorationsProps) {
	const isParticlesInit = useParticlesInit(Boolean(particles));
	const [viewportWidth] = useResize();

	const options = useMemo(
		() => getParticlesOptions(particles, viewportWidth),
		[particles, viewportWidth]
	);

	return (
		<>
			{decorativeIcon && (
				<DecorativeIcon {...decorativeIcon} anchorRef={anchorRef} />
			)}

			{staticDecorations && <StaticDecorations {...staticDecorations} />}

			{particles && isParticlesInit && (
				<ParticlesWrapper>
					<Particles
						key={`header-particles-${viewportWidth}`}
						{...{ options }}
					/>
				</ParticlesWrapper>
			)}
		</>
	);
}

export const HeaderDecorations = memo(HeaderDecorationsInner);
