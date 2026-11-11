import { initParticlesEngine } from '@tsparticles/react';
import { loadSlim } from '@tsparticles/slim';
import { useEffect, useState } from 'react';

export const useParticlesInit = (enabled: boolean) => {
	const [isParticlesInit, setIsParticlesInit] = useState(false);

	useEffect(() => {
		if (!enabled) {
			return;
		}

		initParticlesEngine(async (engine) => {
			await loadSlim(engine);
		}).then(() => setIsParticlesInit(true));
	}, [enabled]);

	return isParticlesInit;
};
