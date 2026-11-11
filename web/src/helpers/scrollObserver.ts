type ScrollListener = (startRowIdx?: number, endRowIdx?: number) => void;

export class ScrollObserver {
	scrollListeners: Set<ScrollListener>;

	constructor() {
		this.scrollListeners = new Set();
	}

	addScrollListener(listener: ScrollListener) {
		this.scrollListeners.add(listener);
	}

	removeScrollListener(listener: ScrollListener) {
		this.scrollListeners.delete(listener);
	}

	notifyScrollListeners(startRowId?: number, endRowId?: number) {
		this.scrollListeners.forEach((listener) => {
			listener(startRowId, endRowId);
		});
	}
}
