# Upstream Sync Analysis: `3502c222`

**Upstream Commit:** `3502c22209a0fc8b86a164063fd9fbb40c6c0e7d`  
**Upstream Repository:** [microsoft/graphrag](https://github.com/microsoft/graphrag/commit/3502c22209a0fc8b86a164063fd9fbb40c6c0e7d)  
**Analyzed:** 2026-03-18T07:01:13Z

---

Manual review complete.

## Summary

Upstream commit `3502c222` updates Python config validation so that, after probing the configured embedding model, GraphRAG automatically realigns `vector_store.vector_size` and each index schema vector dimension to the actual embedding width.

## Dotnet parity

The dotnet codebase does not have a direct `validate_config.py` equivalent yet, so parity is implemented in the immutable configuration models:

- `GraphRagConfig.SyncVectorStoreDimensions(...)` now realigns vector-store dimensions when the configured embed-text model returns a different embedding width.
- `VectorStoreConfig.WithVectorSize(...)` and `IndexSchema.WithVectorSize(...)` propagate the updated dimension consistently.

No additional missed Python parity changes were identified in this upstream commit beyond the vector-size synchronization behavior.
