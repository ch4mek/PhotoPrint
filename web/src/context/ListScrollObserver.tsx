import { createContext, useMemo, useContext, PropsWithChildren } from 'react';

import { ScrollObserver } from '@/helpers/scrollObserver';

const ListScrollObserverContext = createContext(new ScrollObserver());

export function ListScrollObserverProvider({ children }: PropsWithChildren) {
	const contextValue = useMemo(() => new ScrollObserver(), []);

	return (
		<ListScrollObserverContext.Provider value={contextValue}>
			{children}
		</ListScrollObserverContext.Provider>
	);
}

export const useListScrollObserver = () =>
	useContext(ListScrollObserverContext);
