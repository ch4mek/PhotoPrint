import { createContext, PropsWithChildren, useContext, useMemo } from 'react';

import { PrevValueKeeper } from '@/helpers/prevValueKeeper';

const PrevValueKeeperContext = createContext<PrevValueKeeper<string | null>>(
	new PrevValueKeeper(null)
);

export function PrevValueKeeperProvider({ children }: PropsWithChildren) {
	const prevValueKeeperContextValue = useMemo(
		() => new PrevValueKeeper(''),
		[]
	);

	return (
		<PrevValueKeeperContext.Provider value={prevValueKeeperContextValue}>
			{children}
		</PrevValueKeeperContext.Provider>
	);
}

export const usePrevValueKeeperContext = () =>
	useContext(PrevValueKeeperContext);
