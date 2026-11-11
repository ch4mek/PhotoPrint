import { Component, PropsWithChildren } from 'react';

import { PageFallbackContent } from '@/components/PageFallbackContent';

import type { ReactErrorBoundaryState } from './types';

export class ReactErrorBoundary extends Component<
	PropsWithChildren,
	ReactErrorBoundaryState
> {
	constructor(props: PropsWithChildren) {
		super(props);
		this.state = { errorMessage: undefined };
	}

	static getDerivedStateFromError(error: Error) {
		return { errorMessage: error.message };
	}

	render() {
		const { errorMessage } = this.state;
		const { children } = this.props;

		if (errorMessage) {
			return <PageFallbackContent errorMessage={errorMessage} />;
		}

		return children;
	}
}
